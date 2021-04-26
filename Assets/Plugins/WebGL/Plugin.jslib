mergeInto(LibraryManager.library, {
   Set_Lobby: function (_channelName) {
        ReactUnityWebGL.Set_Lobby(Pointer_stringify(_channelName));
   },
   
   Join_Game: function (playerName, avatarId) {
     ReactUnityWebGL.Join_Game(Pointer_stringify(playerName), avatarId);
   },
 
   Change_Choice: function (playerName, choice) {
     ReactUnityWebGL.Change_Choice(Pointer_stringify(playerName), choice);
   },
   
   Create_List: function (factTrue1, factTrue2, falseFact, falsePosition) {
     ReactUnityWebGL.Create_List(factTrue1, factTrue2, falseFact, falsePosition);
   },
  
   Get_Web_Id : function() {
    ReactUnityWebGL.Get_Web_Id();
   },
   
   Web_Log : function(line){
    ReactUnityWebGL.Web_Log(line);
   },
   
   RequestChannelPlayers : function(){
     ReactUnityWebGL.RequestChannelPlayers();    
   },
   
   End_Round: function () {
     ReactUnityWebGL.End_Round();
   },
});
