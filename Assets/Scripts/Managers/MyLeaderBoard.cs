using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyLeaderBoard : MonoBehaviour
{
    const string urlLeaderBoard = "https://heroku-demo-lucentini.herokuapp.com/usuarios/getAllUsers";


    private LeaderBoardData data;

#pragma warning disable 0649
    [SerializeField] private Transform parent;
    [SerializeField] private int maxData;
    [SerializeField] private GameObject playerDataPrefab;
#pragma warning restore 0649
    private int playerId;

    private List<GameObject> leaderboardItems = new List<GameObject>();


    public void Awake()
    {
        PlayerManager.onLoadSuccesfull += SetPlayerId;
    }


    public void SetPlayerId(Profile profile) {
        
        Debug.Log($"# MyLeaderBoard @SetPlayerId {profile.idUser}");
        playerId = profile.idUser;
        
        UpdateUIData();
    }

    public void UpdateUIData() {
        if (!InternetConnectionManager.isOnline) return;

        LoadData();

        if (data.data.Count == 0) return;

        DestoyLeaderboardItems();
        
        var orderedScore = data.data.OrderByDescending(x => int.Parse(x.score)).ToList();

        int myIdPos = orderedScore.IndexOf(orderedScore.Find(x => x.id == playerId));

        int startPos = (int)Mathf.CeilToInt( (float)( (maxData -2) / 2));

        if (myIdPos > 3) {

            for (int i = 0; i < 2; i++)
            {
                var item = orderedScore[i];
                CreateUIItem(item, i);
            }
            for (int i = 0; i < maxData - 2; i++)
            {
                
                int pos = myIdPos - startPos + i;
                if (pos < orderedScore.Count)
                {
                    var item = orderedScore[pos];
                    CreateUIItem(item, pos);
                }
            }
        }
        else{
            for (int i = 0; i < maxData; i++)
            {
                var item = orderedScore[i];
                CreateUIItem(item, i);
            }
        }
    }

    public void DestoyLeaderboardItems() {
        for (int i = 0; i < leaderboardItems.Count; i++) {
            Destroy(leaderboardItems[i]);
        }
    }

    void CreateUIItem(LeaderBoardDataItem item, int pos) {
        GameObject go = GameObject.Instantiate(playerDataPrefab);
        go.transform.SetParent(parent);
        go.GetComponent<UILeaderboardItem>().Set($"#{pos+1} {item.nombre}", item.score ?? "-", Color.white);
        leaderboardItems.Add(go);
    }

    public void LoadData()
    {
        string response = WebRequestHelper.Get(urlLeaderBoard);
        Debug.Log($"#MyLeaderBoard @LoadData: {urlLeaderBoard} \n Response: {response}"  );
        data = response.FromJson<LeaderBoardData>();
    }


}

[System.Serializable]
public class LeaderBoardDataItem {
    public int id;
    public string nombre;
    public string score;

}
[System.Serializable]
public class LeaderBoardData
{
   public List<LeaderBoardDataItem> data = new List<LeaderBoardDataItem>();

}
