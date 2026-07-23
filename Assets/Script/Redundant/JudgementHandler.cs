using System;
using Script.Input;
using UnityEngine;

namespace Script.Core
{
    public class JudgementHandler : MonoBehaviour
    {
        Player _player1;
        Player _player2;
        SituationPlayer _situationPlayer;

        private void Judge()
        { 
            if (_player1 == null) throw new ArgumentNullException(nameof(_player1), "Player 1 is missing.");
            if (_player2 == null) throw new ArgumentNullException(nameof(_player2), "Player 2 is missing.");
            if (IsPlayer1Win())
            {
                _situationPlayer.PlayPlayer1Win(GetPlayer1WinSituation());
            }
            else if (IsPlayer2Win())
            {
                _situationPlayer.PlayPlayer2Win(GetPlayer2WinSituation());
            }
            else
            {
                _situationPlayer.PlayDraw(GetDrawSituation());
            }
        }

        int GetDrawSituation()
        {
            return _player1.stat switch
            {
                Player.Stat.Attack when _player2.stat == Player.Stat.Attack => 1,
                Player.Stat.Charge when _player2.stat == Player.Stat.Charge => 2,
                Player.Stat.Charge when _player2.stat == Player.Stat.Counter => 3,
                Player.Stat.Counter when _player2.stat == Player.Stat.Charge => 4,
                Player.Stat.Counter when _player2.stat == Player.Stat.Counter => 5,
                _ => 0
            };
        }

        int GetPlayer1WinSituation()
        {
            return _player1.stat switch
            {
                Player.Stat.Attack when _player2.stat == Player.Stat.Charge => 1,
                Player.Stat.Attack when _player2.stat == Player.Stat.Attack && _player1.chargeLevel > _player2.chargeLevel => 2,
                Player.Stat.Counter when _player2.stat == Player.Stat.Attack => 3,
                _ => 0
            };
        }
        bool IsPlayer1Win() => GetPlayer1WinSituation() > 0;
        
        int GetPlayer2WinSituation()
        {
            return _player2.stat switch
            {
                Player.Stat.Attack when _player1.stat == Player.Stat.Charge => 1,
                Player.Stat.Attack when _player2.stat == Player.Stat.Attack && _player2.chargeLevel > _player1.chargeLevel => 2,
                Player.Stat.Counter when _player1.stat == Player.Stat.Attack => 3,
                _ => 0
            };
        }
        private bool IsPlayer2Win() => GetPlayer2WinSituation() > 0;
    }

    public class SituationPlayer : MonoBehaviour
    {
        [SerializeField] BaseInputReader inputReader;
        public void PlayPlayer1Win(int getPlayer1WinSituation)
        {
            throw new NotImplementedException();
        }

        public void PlayPlayer2Win(int getPlayer2WinSituation)
        {
            throw new NotImplementedException();
        }

        public void PlayDraw(int getDrawSituation)
        {
            throw new NotImplementedException();
        }
    }
}