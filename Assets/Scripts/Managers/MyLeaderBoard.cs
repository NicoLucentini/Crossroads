using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyLeaderBoard : MonoBehaviour
{
    public const string urlLeaderBoard = "https://heroku-demo-lucentini.herokuapp.com/usuarios/getAllUsers";


    private LeaderBoardData data;

#pragma warning disable 0649
    [SerializeField] private Transform parent;
    [Range(5, 10)][SerializeField] private int maxData = 8;
    [SerializeField] private GameObject playerDataPrefab;
#pragma warning restore 0649
    private int playerId;

    private List<GameObject> leaderboardItems = new List<GameObject>();

    bool leaderboardEnabled = false;
    [SerializeField] private GameObject leaderboardGo;
    [SerializeField] private GameObject leaderboardToggle;
    public void Awake()
    {
        if(OnlineService.isOnline)
            Activate();
    }

    void Activate()
    {
        PlayerManager.onLoadSuccesfull += SetPlayerId;
        leaderboardGo.SetActive(true);
        leaderboardToggle.SetActive(true);
    }

    public void EnableLeaderboard() {
        leaderboardEnabled = !leaderboardEnabled;
    }

    private void SetPlayerId(Profile profile) {
        
        Debug.Log($"# MyLeaderBoard @SetPlayerId {profile.idUser}");
        playerId = profile.idUser;
        
        UpdateUIData();
    }

    public void UpdateUIData() {
        if (!InternetConnectionManager.isOnline || !OnlineService.isOnline) return;

        LoadData();

        if (data.data.Count == 0) return;

        var orderedScore = data.data.OrderByDescending(x => int.Parse(x.score)).ToList();

        maxData = Mathf.Min(maxData, orderedScore.Count);

        int myIdPos = orderedScore.IndexOf(orderedScore.Find(x => x.id == playerId));
        Debug.Log("My pos " + myIdPos);
        List<LeaderBoardDataItem> finalOrdered = new List<LeaderBoardDataItem>();

        if (myIdPos < maxData)
        {
            finalOrdered.AddRange(orderedScore.Take(maxData));
        }
        else{

            finalOrdered.AddRange(orderedScore.Take(3)); // top

            //Estoy en los ultimos 2
            if (orderedScore.Count - myIdPos < 2)
            {
                finalOrdered.AddRange(orderedScore.GetRange(orderedScore.Count - 5, 5));
            }
            else {
                finalOrdered.AddRange(orderedScore.GetRange(myIdPos - 2, 5));
            }
        }
        var it = finalOrdered.Find(x => x.id == playerId);
        int finalPos = finalOrdered.IndexOf(it);
        DrawUI(finalOrdered,orderedScore, finalPos);
    }
    private void DrawUI(List<LeaderBoardDataItem> items, List<LeaderBoardDataItem> order, int myId) {
        DestroyLeaderboardItems();
        for (int i = 0; i < items.Count; i++) {
            CreateUIItem(items[i], order.IndexOf(items[i]), i == myId);
        }
    }

    private void DestroyLeaderboardItems() {
        for (int i = 0; i < leaderboardItems.Count; i++)
            Destroy(leaderboardItems[i]);
    }

    void CreateUIItem(LeaderBoardDataItem item, int pos, bool me) {
        GameObject go = GameObject.Instantiate(playerDataPrefab);
        go.transform.SetParent(parent);
        go.GetComponent<UILeaderboardItem>().Set($"#{pos+1} {item.nombre}", item.score ?? "-",  me ? Color.blue : Color.white);
        leaderboardItems.Add(go);
    }

    public void LoadData()
    {
        data = OnlineService.GetAllPlayers();
        Debug.Log($"#MyLeaderBoard @LoadData: {urlLeaderBoard} \n Response: {  data.data.ListFull()}");
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
