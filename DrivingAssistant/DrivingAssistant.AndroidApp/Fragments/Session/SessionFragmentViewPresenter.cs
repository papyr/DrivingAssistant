﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Fragments.Session
{
    public class SessionFragmentViewPresenter : ViewPresenter
    {
        private readonly User _user;
        private readonly SessionService _sessionService = new SessionService();
        private readonly VideoService _videoService = new VideoService();

        private IEnumerable<DrivingSession> _sessions;
        private int _selectedPosition;
        private View _selectedView;

        //============================================================
        public SessionFragmentViewPresenter(Context context, User user)
        {
            _context = context;
            _user = user;
        }

        //============================================================
        public async Task RefreshDataSource()
        {
            _sessions = await _sessionService.GetByUserAsync(_user.Id);
            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Refresh, _sessions));
        }

        //============================================================
        public void AddButtonClick()
        {
            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Add, null));
        }

        //============================================================
        public void DeleteButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Modify, new Exception("No session selected!")));
                return;
            }

            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var session = _sessions.ElementAt(_selectedPosition);
                try
                {
                    await _sessionService.DeleteAsync(session.Id);
                    Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Delete, true));
                }
                catch (Exception ex)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Delete, ex));
                }
                await RefreshDataSource();
            });

            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public void ModifyButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Modify, new Exception("No session selected!")));
                return;
            }

            var session = _sessions.ElementAt(_selectedPosition);

            if (session.Status != SessionStatus.Unprocessed)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Modify, new Exception("Session already submitted!")));
                return;
            }

            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Modify, session));
        }

        //============================================================
        public void MapButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Map, new Exception("No session selected!")));
                return;
            }

            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Map, _sessions.ElementAt(_selectedPosition)));
        }

        //============================================================
        public async Task OriginalButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Original, new Exception("No session selected!")));
                return;
            }

            var session = _sessions.ElementAt(_selectedPosition);
            var originalVideos = session.Status == SessionStatus.Processed
                ? (await _videoService.GetVideoBySessionAsync(session.Id)).Where(x => x.IsProcessed())
                : await _videoService.GetVideoBySessionAsync(session.Id);
            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Original, originalVideos));
        }

        //============================================================
        public async Task ProcessedButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Map, new Exception("No session selected!")));
                return;
            }

            var session = _sessions.ElementAt(_selectedPosition);

            if (session.Status == SessionStatus.Unprocessed)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Processed, new Exception("Session has not been processed yet!")));
                return;
            }

            var processedVideos = (await _videoService.GetVideoBySessionAsync(session.Id)).Where(x => !x.IsProcessed());
            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Processed, processedVideos));
        }

        //============================================================
        public async Task SubmitButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Submit, new Exception("No session selected!")));
                return;
            }

            var session = _sessions.ElementAt(_selectedPosition);

            if (session.Status != SessionStatus.Unprocessed)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Submit, new Exception("Session already submitted!")));
                return;
            }

            try
            {
                await _sessionService.SubmitAsync(session.Id, ProcessingAlgorithmType.Lane_Departure_Warning);
                await Task.Delay(2000);
                await RefreshDataSource();
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Submit, true));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_Submit, ex));
            }
        }

        //============================================================
        public void ItemClick(int position, View view)
        {
            _selectedView?.SetBackgroundResource(0);
            _selectedPosition = position;
            _selectedView = view;
            Notify(new NotificationEventArgs(NotificationCommand.SessionFragment_ItemClick, view));
        }
    }
}