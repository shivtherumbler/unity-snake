mergeInto(LibraryManager.library,{

  fetchDevice: function(){
    fullscreenButton.click();
    if(isPhone){
      window.screen.orientation.lock("landscape");
    }
    gameInstance.SendMessage("JSLibManager", "isPhone",isPhone.toString() );
  },
  setFullScreen: function(){
    fullscreenButton.click();
  }
});