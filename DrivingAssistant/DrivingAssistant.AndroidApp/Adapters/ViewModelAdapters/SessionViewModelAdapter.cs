﻿using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Java.Lang;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class SessionViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly ICollection<Session> _sessions;

        //============================================================
        public SessionViewModelAdapter(Activity activity, ICollection<Session> sessions)
        {
            _activity = activity;
            _sessions = sessions;
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

            var currentSession = _sessions.ElementAt(position);

            textDescription.Text = "Name: " + currentSession.Name;
            textStartDateTime.Text = "Start: " + currentSession.StartDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            textEndDateTime.Text = "End: " + currentSession.EndDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            textStartCoordinates.Text = "Start position: " + currentSession.StartLocation.X + ", " + currentSession.StartLocation.Y;
            textEndCoordinates.Text = "End position: " + currentSession.EndLocation.X + ", " + currentSession.EndLocation.Y;

            if (currentSession.Processed)
            {
                textProcessed.Text = "PROCESSED";
                textProcessed.SetTextColor(Color.Green);
            }
            else
            {
                textProcessed.Text = "NOT PROCESSED";
                textProcessed.SetTextColor(Color.Red);
            }

            return view;
        }
    }
}