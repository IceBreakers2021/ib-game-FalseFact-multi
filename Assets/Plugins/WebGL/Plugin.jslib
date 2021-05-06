mergeInto(LibraryManager.library, {
   Set_Lobby: function (_channelName) {
        ReactUnityWebGL.Set_Lobby(Pointer_stringify(_channelName));
   },
   
   Join_Game: function (playerName, avatarId, hasTold, selectedFact) {
     ReactUnityWebGL.Join_Game(Pointer_stringify(playerName), avatarId, hasTold, selectedFact);
   },
 
   Change_Choice: function (playerWebId, choice) {
     ReactUnityWebGL.Change_Choice(Pointer_stringify(playerWebId), choice);
   },
  
   Get_Web_Id : function() {
    ReactUnityWebGL.Get_Web_Id();
   },
   
   Tell_End_Game : function() {
    ReactUnityWebGL.Tell_End_Game();
   },
      
   Web_Log : function(line){
    ReactUnityWebGL.Web_Log(Pointer_stringify(line));
   },
   
   Tell_Next_Round : function(playerId){
    ReactUnityWebGL.Tell_Next_Round(Pointer_stringify(playerId));
   },
   
   RequestChannelPlayers : function(){
     ReactUnityWebGL.RequestChannelPlayers();    
   },
   Ask_For_Current_Teller: function(){
     ReactUnityWebGL.Ask_For_Current_Teller();
   },
   
   Reply_Current_Teller: function(webid, falsePos, false1, true1, true2){
     ReactUnityWebGL.Reply_Current_Teller(Pointer_stringify(webid), falsePos, 
       Pointer_stringify(false1), Pointer_stringify(true1), Pointer_stringify(true2));
   },
   End_Round: function () {
     ReactUnityWebGL.End_Round();
   },
});
