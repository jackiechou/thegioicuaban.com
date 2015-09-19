AVIM JavaScript Vietnamese Input Method TinyMCE plugin
======================================================

Project is hosted at Sourceforge, http://sourceforge.net/projects/tinymcevim. See http://mohanjith.com/2008/04/vietnamese-input-plugin-for-tinymce.html
for more details.

Requirements
------------
 1. TinyMCE - http://tinymce.moxiecode.com/

Installation
-------------
 1. Place the vietunicode directory in jscripts/tiny_mce/plugins directory of you TinyMCE installation. Directory
 structure should look like bellow.

 - jscripts/
 |- ...
 |- plugins/
 |-- ...
 |-- vietunicode/
 |-- ...
 |- ..

 2. Add vietunicode to the list of plugins in the tinyMCE.init object. (Specified in the file you
 are using TinyMCE in)

  e.g.
	tinyMCE.init({
		mode : "textareas",
		theme : "simple",
		plugins : "vietunicode"
	});

 3. Add vietunicode_method_listbox to theme options. Specified in the file you are using TinyMCE in)

  e.g.
  	tinyMCE.init({
		mode : "textareas",
		theme : "simple",
		plugins : "vietunicode",
		theme_advanced_buttons1 : "vietunicode_method_listbox"
	});

 If you find step 2 and 3 confusing please send me the file you are using TinyMCE in.

Changing captions
-----------------
All captions relating to the plugin are found in the file vietunicode/langs/en.js. The file is a
standard TinyMCE plug-in language file.

  e.g.
	tinyMCE.addI18n({en:{vietunicode:{
		select_input_method:"Gõ Tiếng Việt",
		off_desc:"Normal",
		telex_desc:"Telex",
		vni_desc:"VNI",
		viqr_desc:"VIQR",
		auto_desc:"Tự Động"
	}}});

-- Mohanjith (http://mohanjith.com/ | moha at users.sourceforge.net)
