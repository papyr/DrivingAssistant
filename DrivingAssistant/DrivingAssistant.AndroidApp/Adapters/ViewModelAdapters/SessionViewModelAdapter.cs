﻿using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Java.Lang;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class SessionViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<DrivingSession> _sessions;

        //============================================================
        public SessionViewModelAdapter(Activity activity, object sessions)
        {
            _activity = activity;
            _sessions = sessions as List<DrivingSession>;
        }

        //============================================================
        public override int Count => _sessions.Count;

        //============================================================
        public override Object GetItem(int position)
        {
            return null;
        }

        //============================================================
        public override long GetItemId(int position)
        {
            return _sessions.ElementAt(position).Id;
        }

        //============================================================
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_session_list, parent, false);
            var textDescription = view.FindViewById<TextView>(Resource.Id.sessionTextDescription);
            var textStartDateTime = view.FindViewById<TextView>(Resource.Id.sessionTextStartDateTime);
            var textEndDateTime = view.FindViewById<TextView>(Resource.Id.sessionTextEndDateTime);
            var textStartCoordinates = view.FindViewById<TextView>(Resource.Id.sessionTextStartCoordinates);
            var textEndCoordinates = view.FindViewById<TextView>(Resource.Id.sessionTextEndCoordinates);
            var textProcessed = view.FindViewById<TextView>(Resource.Id.sessionTextProcessed);
            var progressBar = view.FindViewById<ProgressBar>(Resource.Id.sessionProgressBar);

            var currentSession = _sessions.ElementAt(position);

            textDescription.Text = "Name: " + currentSession.Name;
            textStartDateTime.Text = "Start: " + currentSession.StartDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            textEndDateTime.Text = "End: " + currentSession.EndDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            textStartCoordinates.Text = "Start position: " + currentSession.StartLocation.X + ", " + currentSession.StartLocation.Y;
            textEndCoordinates.Text = "End position: " + currentSession.EndLocation.X + ", " + currentSession.EndLocation.Y;

            switch (currentSession.Status)
            {
                case SessionStatus.Processed:
                {
                    textProcessed.Text = "PROCESSED";
                    textProcessed.SetTextColor(Color.Green);
                    progressBar.Visibility = ViewStates.Gone;
                    break;
                }
                case SessionStatus.Unprocessed:
                {
                    textProcessed.Text = "NOT PROCESSED";
                    textProcessed.SetTextColor(Color.Yellow);
                    progressBar.Visibility = ViewStates.Gone;
                    break;
                }
                case SessionStatus.Failed:
                {
                    textProcessed.Text = "FAILED";
                    textProcessed.SetTextColor(Color.Red);
                    progressBar.Visibility = ViewStates.Gone;
                    break;
                }
                case SessionStatus.Processing:
                {
                    textProcessed.Text = "PROCESSING";
                    textProcessed.SetTextColor(Color.Blue);
                    progressBar.Visibility = ViewStates.Visible;
                    break;
                }
            }

            return view;
        }
    }
}