# GuitLong

Is an interactive playlong guitar practice app, that can help others practice certain song.
Enter an URL from https://www.ultimate-guitar.com, from a specific song.

Import and then edit the Strumming pattern and BPM of the sections. Save
Then play the song with interactive guitar chords.

The app is built with C# and WPF on .NET 10. It uses HtmlAgilityPack for working with song pages and PDFsharp for features.
Using chord images from: https://emusic.tools/en/tools/chords

This is but a fun project, these kinds of applications are often paid, so why not make my own?

## Requirements

- Windows
- .NET 10 SDK to run from source

In the directory run:
```bash
dotnet run --project GuitLong/GuitLong.csproj
