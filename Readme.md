# Translation System

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/victorafael-com/unitypkg-translation-system/blob/main/LICENSE)

Simple translation system for Unity

## Installation

To install this project use [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), following those steps:

1. Open Unity Package on `Window/Package Manager`;
2. Click the + button;
3. Choose `Add package from git URL...`;
4. Add the url `https://github.com/victorafael-com/unitypkg-translation-system.git`

## Resources

### TranslatedString

Create a property of type TranslatedString

```
public TranslatedString openText;
```

To get the translated value, use the property Value:

```
label.text = openText.Value;
```

On Editor, there's a button to open the Translation Window and select the desired key.

(Sorry, Better documentation will be provided later)
