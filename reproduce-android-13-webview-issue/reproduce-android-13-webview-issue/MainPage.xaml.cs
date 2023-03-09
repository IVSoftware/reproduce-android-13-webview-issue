using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace reproduce_android_13_webview_issue
{
    public partial class MainPage : ContentPage
    {
        SemaphoreSlim _noReentry = new SemaphoreSlim(1, 1);
        public MainPage()
        {
            InitializeComponent();
            // rteditor.Source = TestHtml01; // PASS
            rteditor.Source = TestHtml02; // FAIL
            // rteditor.Source = TestHtml03; // FAIL
            // rteditor.Source = TestHtml04;  // PASS

            rteditor.PropertyChanged += onPropertyChanged;
        }

        private async void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                await _noReentry.WaitAsync();

                switch (e.PropertyName)
                {
                    case nameof(rteditor.SelectionRange):
                        Debug.WriteLine($"PROPERTY CHANGED: {e.PropertyName} Start: {rteditor.SelectionRange.Start} End: {rteditor.SelectionRange.End}");
                        Debug.WriteLine(await rteditor.GetHtmlAsync());
                        break;
                    default:
                        Debug.WriteLine($"PROPERTY CHANGED: {e.PropertyName}");
                        Debug.WriteLine(await rteditor.GetHtmlAsync());
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            finally
            {
                _noReentry.Release();
            }
        }

        public const string TestHtml01 =
@"<html>
	<body>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
        <p>Lore mi'ps umdol or si tame tc onse ct ETU etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq ue.</p>
    </body>
</html>";

        // Paragraph 2 is one char longer
        public const string TestHtml02 =
@"<html>
	<body>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
        <p>Lore mi'ps umdol or si tame tc onse ct ETU etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq uet.</p>
    </body>
</html>";

        // Replace single quote with 'x' in the shorter string makes it fail.
        public const string TestHtml03 =
@"<html>
	<body>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
        <p>Lore mixps umdol or si tame tc onse ct ETU etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq ue.</p>
    </body>
</html>";

        // Replace 'ETU' with 'etu' in the longer string makes it pass.
        public const string TestHtml04 =
@"<html>
	<body>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
        <p>Lore mi'ps umdol or si tame tc onse ct etu etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq uet.</p>
    </body>
</html>";
    }
}
