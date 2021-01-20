
using QSearcher_.Data;
using QSearcher_.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace QSearcher_.Controls
{
    class EventsSearchHandler : SearchHandler
    {
        public IList<Event> Events { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = Events
                    .Where(ev => ev.Title.ToLower().Contains(newValue.ToLower()))
                    .ToList<Event>();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            MyListPage.ev.Navigator((Event)item);
        }
    }
}

