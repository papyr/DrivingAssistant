﻿using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SettingsFragment : Fragment
    {
        private readonly User _user;
        private UserSettings _userSettings;

        private readonly UserSettingsService _userSettingsService = new UserSettingsService();
        private readonly SessionService _sessionService = new SessionService();

        private TextView _textCameraSession;
        private TextView _textCameraHost;
        private TextView _textCameraUsername;
        private TextView _textCameraPassword;
        private TextView _textCameraStatus;

        private Button _buttonStartRecording;
        private Button _buttonStopRecording;
        private Button _buttonSave;

        //============================================================
        public SettingsFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);
            SetupFragmentFields(view);
            PopulateFields();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _textCameraSession = view.FindViewById<TextView>(Resource.Id.settingsTextCameraSession);
            _textCameraHost = view.FindViewById<TextView>(Resource.Id.settingsTextCameraHost);
            _textCameraUsername = view.FindViewById<TextView>(Resource.Id.settingsTextCameraUsername);
            _textCameraPassword = view.FindViewById<TextView>(Resource.Id.settingsTextCameraPassword);
            _textCameraStatus = view.FindViewById<TextView>(Resource.Id.settingsTextRecordingStatus);

            _buttonStartRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStartRecording);
            _buttonStopRecording = view.FindViewById<Button>(Resource.Id.settingsButtonStopRecording);
            _buttonSave = view.FindViewById<Button>(Resource.Id.settingsButtonSave);

            _textCameraSession.Click += OnTextCameraSessionClick;
            _buttonStartRecording.Click += OnStartRecordingClick;
            _buttonStopRecording.Click += OnStopRecordingClick;
            _buttonSave.Click += OnButtonSaveClick;
        }

        //============================================================
        private async void PopulateFields()
        {
            try
            {
                _userSettings = await _userSettingsService.GetByUserAsync(_user.Id);
                _textCameraHost.Text = _userSettings.CameraHost;
                _textCameraUsername.Text = _userSettings.CameraUsername;
                _textCameraPassword.Text = _userSettings.CameraPassword;
                try
                {
                    _textCameraStatus.Text = await _userSettingsService.GetRecordingStatus(_user.Id);
                }
                catch (Exception)
                {
                    _textCameraStatus.Text = "Failed to retrieve status!";
                }
                if (_userSettings.CameraSessionId != -1)
                {
                    try
                    {
                        var cameraSession = await _sessionService.GetByIdAsync(_userSettings.CameraSessionId);
                        _textCameraSession.Text = cameraSession.Name;
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Context, "Failed to populate some fields!\n" + ex.Message, ToastLength.Long).Show();
                        _textCameraSession.Text = "None";
                    }
                }
                else
                {
                    _textCameraSession.Text = "None";
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, "Failed to retrieve settings!\n" + ex.Message, ToastLength.Long).Show();
            }
        }

        //============================================================
        private async void OnTextCameraSessionClick(object sender, EventArgs e)
        {
            try
            {
                var availableSessions = (await _sessionService.GetByUserAsync(_user.Id)).Where(x => x.Status == SessionStatus.Unprocessed);
                var sessionStringList = availableSessions.Select(x => x.Name).ToList();
                sessionStringList.Add("None");
                var alert = new AlertDialog.Builder(Context);
                alert.SetTitle("Select Camera Session");
                alert.SetItems(sessionStringList.ToArray(), (o, args) =>
                {
                    if (args.Which == sessionStringList.Count - 1)
                    {
                        _userSettings.CameraSessionId = -1;
                        _textCameraSession.Text = "None";
                    }
                    else
                    {
                        var selectedSession = availableSessions.ElementAt(args.Which);
                        _userSettings.CameraSessionId = selectedSession.Id;
                        _textCameraSession.Text = selectedSession.Name;
                    }
                });

                alert.Create().Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, "Failed to populate session list!\n" + ex.Message, ToastLength.Short).Show();
            }
        }

        //============================================================
        private bool Validate()
        {
            var cameraIp = _textCameraHost.Text;
            if (!cameraIp.Contains('.'))
            {
                return false;
            }

            if (cameraIp.Split('.').Length != 5)
            {
                return false;
            }

            return true;
        }

        //============================================================
        private async void OnButtonSaveClick(object sender, EventArgs e)
        {
            /*if (!Validate())
            {
                Toast.MakeText(Context, "The data is not valid!!", ToastLength.Short).Show();
            }*/
            _userSettings.CameraHost = _textCameraHost.Text;
            _userSettings.CameraUsername = _textCameraUsername.Text;
            _userSettings.CameraPassword = _textCameraPassword.Text;
            try
            {
                await _userSettingsService.SetAsync(_userSettings);
                Toast.MakeText(Context, "Settings successfully saved!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, "Failed to save settings!\n" + ex.Message, ToastLength.Long).Show();
            }
        }

        //============================================================
        private async void OnStartRecordingClick(object sender, EventArgs e)
        {
            try
            {
                await _userSettingsService.StartRecordingAsync(_user.Id);
            }
            catch (Exception)
            {
                Toast.MakeText(Context, "Failed to start recording! Settings may be incorrect or the recording is already started!", ToastLength.Long).Show();
            }
        }

        //============================================================
        private async void OnStopRecordingClick(object sender, EventArgs e)
        {
            try
            {
                await _userSettingsService.StopRecordingAsync(_user.Id);
            }
            catch (Exception)
            {
                Toast.MakeText(Context, "Failed to stop recording! Settings may be incorrect or the recording is already stopped!", ToastLength.Long).Show();
            }
        }
    }
}