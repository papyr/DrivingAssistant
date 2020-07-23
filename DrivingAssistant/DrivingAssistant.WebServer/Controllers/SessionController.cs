﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.ImageProcessing;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Services.Mssql;
using DrivingAssistant.WebServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DrivingAssistant.WebServer.Controllers
{
    [ApiController]
    public class SessionController : ControllerBase
    {
        private ISessionService _sessionService;

        //============================================================
        [HttpGet]
        [Route("sessions")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                Logger.Log(
                    "Received GET sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                _sessionService = ISessionService.NewInstance(typeof(MssqlSessionService));
                var sessions = await _sessionService.GetAsync();
                return Ok(JsonConvert.SerializeObject(sessions, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPost]
        [Route("sessions")]
        public async Task<IActionResult> PostAsync()
        {
            try
            {
                Logger.Log(
                    "Received POST sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = ISessionService.NewInstance(typeof(MssqlSessionService));
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                return Ok(await _sessionService.SetAsync(session));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpPut]
        [Route("sessions")]
        public async Task<IActionResult> PutAsync()
        {
            try
            {
                Logger.Log(
                    "Received PUT sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                using var streamReader = new StreamReader(Request.Body);
                _sessionService = ISessionService.NewInstance(typeof(MssqlSessionService));
                var session = JsonConvert.DeserializeObject<Session>(await streamReader.ReadToEndAsync());
                await _sessionService.SetAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpDelete]
        [Route("sessions")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                Logger.Log(
                    "Received DELETE sessions from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _sessionService = ISessionService.NewInstance(typeof(MssqlSessionService));
                var mediaService = IMediaService.NewInstance(typeof(MssqlMediaService));
                var session = (await _sessionService.GetAsync()).First(x => x.Id == id);
                foreach (var media in (await mediaService.GetAsync()).Where(x => x.SessionId == session.Id))
                {
                    media.SessionId = default;
                    await mediaService.SetAsync(media);
                }
                await _sessionService.DeleteAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }

        //============================================================
        [HttpGet]
        [Route("process_session")]
        public async Task<IActionResult> ProcessSession()
        {
            try
            {
                Logger.Log(
                    "Received POST process_session from :" + Request.HttpContext.Connection.RemoteIpAddress + ":" +
                    Request.HttpContext.Connection.RemotePort, LogType.Info, true);
                var id = Convert.ToInt64(Request.Query["Id"].First());
                _sessionService = ISessionService.NewInstance(typeof(MssqlSessionService));
                using var mediaService = IMediaService.NewInstance(typeof(MssqlMediaService));
                using var reportService = IReportService.NewInstance(typeof(MssqlReportService));
                var session = (await _sessionService.GetAsync()).First(x => x.Id == id);
                var linkedMedia = (await mediaService.GetAsync()).Where(x => x.SessionId == session.Id);
                var imageProcessor = new ImageProcessor(Parameters.Default());
                foreach (var media in linkedMedia.Where(x => !x.IsProcessed()))
                {
                    Media processedMedia;
                    Report report;
                    if (media.Type == MediaType.Image)
                    {
                        var processedFilename = imageProcessor.ProcessImage(media.Filepath, false, out var result);
                        processedMedia = new Media
                        {
                            Type = MediaType.Image,
                            Filepath = processedFilename,
                            Source = media.Source,
                            Description = media.Description,
                            DateAdded = DateTime.Now,
                            Id = -1,
                            ProcessedId = -1,
                            SessionId = media.SessionId,
                            UserId = media.UserId
                        };
                        report = Report.FromImageReport(result, media.Id, session.Id);
                    }
                    else
                    {
                        var processedFilename = imageProcessor.ProcessVideo(media.Filepath, 10, out var result);
                        processedMedia = new Media
                        {
                            Type = MediaType.Video,
                            Filepath = processedFilename,
                            Source = media.Source,
                            Description = media.Description,
                            DateAdded = DateTime.Now,
                            Id = -1,
                            ProcessedId = -1,
                            SessionId = media.SessionId,
                            UserId = media.UserId
                        };
                        report = Report.FromVideoReport(result, media.Id, session.Id);
                    }

                    processedMedia.Id = await mediaService.SetAsync(processedMedia);
                    report.Id = await reportService.SetAsync(report);
                    media.ProcessedId = processedMedia.Id;
                    await mediaService.SetAsync(media);
                }

                session.Processed = true;
                await _sessionService.SetAsync(session);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, LogType.Error, true);
                return Problem(ex.Message);
            }
        }
    }
}
