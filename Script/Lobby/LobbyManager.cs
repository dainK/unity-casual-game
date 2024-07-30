using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameHeaven
{
    public class LobbyManager : MonoBehaviour
    {
        public void Entry1to50()
        {
            SceneManager.LoadScene("Game1to50", LoadSceneMode.Single);
        }
        public void Entry2048()
        {
            SceneManager.LoadScene("Game2048", LoadSceneMode.Single);
        }
        public void Entry3Match()
        {
            SceneManager.LoadScene("Game3Match", LoadSceneMode.Single);
        }
        public void Entry10x10()
        {
            SceneManager.LoadScene("Game10x10", LoadSceneMode.Single);
        }
        public void EntryMerge()
        {
            SceneManager.LoadScene("GameMerge", LoadSceneMode.Single);
        }
        public void EntryMemory()
        {
            SceneManager.LoadScene("GameMemory", LoadSceneMode.Single);
        }
        public void EntryFreeCell()
        {
            SceneManager.LoadScene("GameFreeCell", LoadSceneMode.Single);
        }
    }
}