window.onload=LoadPage;

function LoadPage()
{
	if (Has2Lang())
	{
	    var lang = getCookie("lang");
		
		if (lang){
			document.getElementById("langSelect").value = lang;
			SwitchLanguage(lang);
		}
		
		document.getElementById("languageSelect").style.visibility = "visible";
	}
}

function Has2Lang()
{
	var hasCSharp = false;
	var hasVB = false;
	var spanElements = document.getElementsByTagName("span");
	
	for(var i = 0; i < spanElements.length; ++i)
	{
	    var codeLang = spanElements[i].getAttribute("codeLanguage");
		
		if (codeLang)
		{
			if ("CSharp" == codeLang)
			{
				hasCSharp = true;
			}
			else if ("VisualBasicUsage" == codeLang)
			{
				hasVB = true;
			}
		}
		
		if (hasVB && hasCSharp)
			return true;
	}
	
	return false;
}

function CopyCode(key)
{
	var trElements = document.getElementsByTagName("tr");
	for(var i = 0; i < trElements.length; ++i)
	{
		if(key.parentNode.parentNode.parentNode == trElements[i].parentNode)
		{
			i++;
			if(document.all){
				var text = trElements[i].innerText;
				var lines = trElements[i].getElementsByTagName('b');
				if (lines.length > 0)
				{
					text = lines[0].innerText;
					for(var j = 1; j < lines.length; j++)
					{
						text += '\r\n' + lines[j].innerText;
					}
				}
				if (text != "") {
					window.clipboardData.setData("Text", text);
				}
			} else {
				var text = trElements[i].textContent;
				var lines = trElements[i].getElementsByTagName('b');
				if (lines.length > 0)
				{
					text = lines[0].textContent;
					for(var j = 1; j < lines.length; j++)
					{
						text += '\r\n' + lines[j].textContent;
					}
				}
				if (text != "") {
					CopyToClipboardFF(text);
				}
			}
			break;
		}
	}
}

function CopyToClipboardFF(text){
	//Firefox
	netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');

	var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
	if (!clip) return;

	var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
	if (!trans) return;

	trans.addDataFlavor('text/unicode');

	var str = new Object();
	var len = new Object();

	var str = Components.classes["@mozilla.org/supports-string;1"]
				.createInstance(Components.interfaces.nsISupportsString);

	str.data=text;

	trans.setTransferData("text/unicode",str,text.length*2);

	var clipid = Components.interfaces.nsIClipboard;
	if (!clip) return;

	clip.setData(trans, null, clipid.kGlobalClipboard);
}

function ChangeCopyCodeIcon(key)
{
	var i;
	var imageElements = document.getElementsByName("ccImage")
	var copyImg = document.getElementById("copyImage")
	var copyHoverImg = document.getElementById("copyHoverImage")
	
	for(i=0; i<imageElements.length; ++i)
	{
		if(imageElements[i].parentNode == key)
		{
			if(imageElements[i].src == copyImg.src)
			{
				imageElements[i].src = copyHoverImg.src;
				imageElements[i].alt = copyHoverImg.alt;
			}
			else
			{
				imageElements[i].src = copyImg.src;
				imageElements[i].alt = copyImg.alt;
			}
		}
	}
}

function SwitchLanguage(selectedLang)
{
	var spanElements = document.getElementsByTagName("span");
	
	for(var i = 0; i < spanElements.length; ++i)
	{
	    var codeLang = spanElements[i].getAttribute("codeLanguage");

	    if ((codeLang == "CSharp") || (codeLang == "VisualBasicUsage"))
		{
			if ((selectedLang == "All") || (selectedLang == codeLang))
			{
				spanElements[i].style.display = "block";
			}
			else
			{
				spanElements[i].style.display = "none";
			}
		}
	}
	
	setCookie("lang", selectedLang);
}

function setCookie(name, value, seconds) {
	document.cookie = name+"="+value+"; expires=Thu, 2 Aug 2011 20:00:00 UTC; path=/";
}

function getCookie(name) {
	name = name + "=";
	var carray = document.cookie.split(';');

	for(var i=0;i < carray.length;i++) {
		var c = carray[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(name) == 0) return c.substring(name.length,c.length);
	}

	return null;
}