namespace Damath.EOS.Lobbies
{
    /// <summary>
    /// Class represents RTC State (Voice) of a Lobby
    /// </summary>
    public class LobbyRTCState
    {
        /// <summary>
        /// Is this person currently connected to the RTC room?
        /// </summary>
        public bool IsInRTCRoom = false;
        /// <summary>
        /// Is this person currently talking (audible sounds from their audio output)
        /// </summary>
        public bool IsTalking = false;
        /// <summary>
        /// We have locally muted this person (others can still hear them)
        /// </summary>
        public bool IsLocalMuted = false;
        /// <summary>
        /// Has this person muted their own audio output (nobody can hear them)
        /// </summary>
        public bool IsAudioOutputDisabled = false;
        /// <summary>
        /// Are we currently muting this person?
        /// </summary>
        public bool MuteActionInProgress = false;
        /// <summary>
        /// Has this person enabled press to talk
        /// </summary>
        public bool PressToTalkEnabled = false;
    }
}