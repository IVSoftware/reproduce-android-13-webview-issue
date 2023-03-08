Calling all Xamarin-Android gurus - this issue is way over my head. 

I have a Google Play App Store product in beta testing that is experiencing catastrophic, repeatable, data-loss inducing failures related to Xamarin.Forms.WebView on **Android 13 only**. Feel free to [clone](https://github.com/IVSoftware/reproduce-android-13-webview-issue.git) my minimal reproducible example project. There are branches for the `Telerik.RadRichTextEditor` and the `Syncfusion.SfRichTextEditor` which as I understand it both wrap the WebView as a last stop before java and the native control and other things I know little about. I want to emphasize that on my Android 12 physical device there are no issues whatsoever.

The testbench is simple: the single page has an editor on it, and we load some html into that editor.

    public MainPage()
    {
        InitializeComponent();
        // rteditor.Source = TestHtml01; // PASS
        rteditor.Source = TestHtml02; // FAIL
    }

The issue occurs when control is tapped and the soft keyboard opens. Whether it works or not depends on the html. This source behaves _correctly_ when the editor is tapped at the end of the text to set the cursor and start editing.

    public const string TestHtml01 =
    @"<html>
	    <body>
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
            <p>Lore mi'ps umdol or si tame tc onse ct ETU etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq ue.</p>
        </body>
    </html>";

[log-pass-telerik.txt]

However, add just one character to the end of the html and _the entire document is erased_ as the WebView attempts to scroll to the character position. 

    // In this source, a single character is added at the end of paragraph 2.
    public const string TestHtml02 =
    @"<html>
	    <body>
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante id eros aliquet faucibus.</p>
            <p>Lore mi'ps umdol or si tame tc onse ct ETU etur adi piscinge lita liqu ams agitt is a NteIderOsew aliq uet.</p>
        </body>
    </html>";

![screenshot]()
[log-fail-telerik.txt]

With the `SfRichTextEditor`, the first html source still behaves normally. The html with the extra character still alters the document data, but in this case at least it doesn't destroy it all.

***
**Root Cause v Sniff Test**

I don't know what's happening here and am hoping that someone with knowledge of the internals can offer:

- A solution.
- A workaround.
- Any explanation.

I _do_ know what it _smells_ like, however. I believe that:
- The WebView is attempting to ScrollToPosition when the soft input opens and the height changes.
- Perhaps there is some string measurement where the character index can be converted to view port coordinates.

Here's why I think that:

I ruled out that it's the _length_ of the text! What I can do is change the upper case 'ETU' in the failing source to a lower-case 'etu' and then it passes. To me this seems as though the lower case is _narrower_ in the proportional font and this makes a difference even though the character count has _not_ changed.




