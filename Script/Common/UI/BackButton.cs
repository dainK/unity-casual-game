using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace GameHeaven
{
    public class BackButton : MonoBehaviour
    {
        //private Button _backButton;

        private void Start()
        {
            //_backButton = GetComponent<Button>();
            //_backButton.onClick.AddListener(() =>
            //{
            //    SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);

            //});
        }

        public void Back()
        {
            SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
    }
}