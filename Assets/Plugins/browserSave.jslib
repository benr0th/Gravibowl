mergeInto(LibraryManager.library, {
	GetData: function(yourkey) {
		if (localStorage.getItem(Pointer_stringify(yourkey)) !== null) {
			var returnStr = localStorage.getItem(Pointer_stringify(yourkey));
			var bufferSize = lengthBytesUTF8(returnStr) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(returnStr, buffer, bufferSize);
			return buffer;
		}
	},
	SetData: function(yourkey, yourdata) {
		localStorage.setItem(Pointer_stringify(yourkey), Pointer_stringify(yourdata));
	}
});