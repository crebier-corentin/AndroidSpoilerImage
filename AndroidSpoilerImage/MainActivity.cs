using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using Android.Provider;
using Android.Support.V4.Content;
using Java.IO;
using Uri = Android.Net.Uri;


namespace AndroidSpoilerImage
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    [IntentFilter(new[] {Intent.ActionSend}, Categories = new[] {Intent.CategoryDefault}, DataMimeType = "image/*")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Intent.Action == Intent.ActionSend)
            {
                var uri = Intent.Extras.Get(Intent.ExtraStream) as Uri;
                var from = new File(uri.Path);

                var toFilepath = $"{GetExternalCacheDirs()[0].AbsolutePath}{File.Separator}SPOILER_{from.Name}";

                //Copy
                using (var input = ContentResolver.OpenInputStream(uri))
                using (var output =  new System.IO.FileStream(toFilepath, System.IO.FileMode.Create))
                {
                    input.CopyTo(output);
                }

                //Share
                var toUri = FileProvider.GetUriForFile(this, Application.Context.PackageName + ".provider", new File(toFilepath));
                SendImage(toUri);
            }

            Finish();
        }

       
      
        private void SendImage(Uri imageUri)
        {
            var sharingIntent = new Intent();
            sharingIntent.SetAction(Intent.ActionSend);
            sharingIntent.SetType("image/*");
            sharingIntent.PutExtra(Intent.ExtraStream, imageUri);
            sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantPersistableUriPermission);
            StartActivity(Intent.CreateChooser(sharingIntent, "Share to Discord"));
        }
    }
}