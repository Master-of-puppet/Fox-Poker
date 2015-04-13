using Puppet.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface ILobbyView : IView
{
    void DrawChannels(List<DataChannel> channels);
    void DrawLobbies(List<DataLobby> lobbies);
    void RemoveLobby(DataLobby lobby);
    void UpdateLobby(DataLobby lobby);
    void AddLobby(DataLobby lobby);
}

