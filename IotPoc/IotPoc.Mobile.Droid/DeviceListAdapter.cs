using System.Collections.Generic;
using Android.App;
using Android.Widget;

namespace IotPoc.Mobile.Droid.ApplicationLayer
{
    /// <summary>
    /// Adapter that presents Tasks in a row-view
    /// </summary>
    public class DeviceListAdapter : BaseAdapter<string>
    {
        Activity _context = null;
        IList<string> _tasks = new List<string>();

        public DeviceListAdapter(Activity context, IList<string> tasks) : base()
        {
            _context = context;
            _tasks = tasks;
        }

        public override string this[int position]
        {
            get { return _tasks[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return _tasks.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            // Get our object for position
            string device = _tasks[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // gives us some performance gains by not always inflating a new view
            // will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()

            //			var view = (convertView ?? 
            //					context.LayoutInflater.Inflate(
            //					Resource.Layout.TaskListItem, 
            //					parent, 
            //					false)) as LinearLayout;
            //			// Find references to each subview in the list item's view
            //			var txtName = view.FindViewById<TextView>(Resource.Id.NameText);
            //			var txtDescription = view.FindViewById<TextView>(Resource.Id.NotesText);
            //			//Assign item's values to the various subviews
            //			txtName.SetText (item.Name, TextView.BufferType.Normal);
            //			txtDescription.SetText (item.Notes, TextView.BufferType.Normal);

            // TODO: use this code to populate the row, and remove the above view
            var view = (convertView ??
                _context.LayoutInflater.Inflate(
                    Android.Resource.Layout.SimpleListItemChecked,
                    parent,
                    false)) as TextView;
            view.SetText(device.ToString(), TextView.BufferType.Normal);


            //Finally return the view
            return view;
        }
    }
}