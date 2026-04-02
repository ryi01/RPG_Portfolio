
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // 퀘스트 원본 DB
    // 모든 QuestData 등록 
    [SerializeField] private List<QuestData> questDatabase = new List<QuestData>();
    [SerializeField] private int firstQuestID = 1001;

    // 현재 진행중인 퀘스트
    private List<QuestInstance> activeQuest = new List<QuestInstance>();
    // 진행 완료된 퀘스트
    private List<QuestInstance> completeQuest = new List<QuestInstance>();

    // QuestId로 QuestData 조회를 위한 딕셔너리 
    private Dictionary<int, QuestData> questDict = new Dictionary<int, QuestData>();
    // 현재 수락 가능한 퀘스트 ID 목록 
    private HashSet<int> availableQuestIDs = new HashSet<int>();

    public static Action OnQuestUpdate;
    public static Action<QuestData> OnQuestCompleted;

    // 저장된 퀘스트 상태를 메모리에 캐싱 용도 => 퀘스트 ID로 퀘스트 저장 데이터 저장 
    private Dictionary<int, PlayerQuestSaveData> questStateMap = new Dictionary<int, PlayerQuestSaveData>();

    private void Awake()
    {
        // 퀘스트 DB를 퀘스트 딕셔너리로 변경
        BuildDictionary();
        // 첫 퀘스트 수락 
        availableQuestIDs.Add(firstQuestID);
    }
    private void Start()
    {
        // 현재 DB를 확인 후 로드
        LoadQuestFromDB();
    }

    // 퀘스트 DB 리스트를 QuestDict로 구성
    private void BuildDictionary()
    {
        questDict.Clear();

        foreach(var quest in questDatabase)
        {
            if (quest == null) continue;
            if (questDict.ContainsKey(quest.QuestID)) continue;
            questDict.Add(quest.QuestID, quest);
        }
    }
    // 퀘스트가 수락 가능한지 확인
    public bool IsQuestAvailable(QuestData data)
    {
        if (data == null) return false;
        return availableQuestIDs.Contains(data.QuestID);
    }
    // 특정 퀘스트의 현재 상태 반환
    public EnumTypes.QUEST GetQuestState(QuestData data)
    {
        if (data == null) return EnumTypes.QUEST.NOT_START;
        // 진행되는 퀘스트 중에서 실행가능한 퀘스트 찾기
        var active = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        // 찾으면 진행 중인 퀘스트 
        if (active != null) return active.State;
        // 완료 목록에서 찾기 
        var complete = completeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        // 찾았으면 완료된 상태의 퀘스트
        if (complete != null) return complete.State;
        // 둘 다 아니면 시작 안한 상태
        return EnumTypes.QUEST.NOT_START;
    }

    // 퀘스트 시작
    public bool StartQuest(QuestData data)
    {
        if (data == null) return false;
        // 진행중이거나 완료 됐는지 확인 후, 시작
        if (activeQuest.Exists(q => q.Data != null && q.Data.QuestID == data.QuestID)) return false;
        if (completeQuest.Exists(q => q.Data != null && q.Data.QuestID == data.QuestID)) return false;

        // 새 퀘스트 인스턴스 생성 후, 진행 목록에 추가
        QuestInstance instance = new QuestInstance(data);
        activeQuest.Add(instance);
        // DB 저장 (currentCount, isAccepted, isCompleted, isReward)
        SaveQuestState(data.QuestID, 0, 1, 0, 0);

        OnQuestUpdate?.Invoke();
        return true;
    }
    // 퀘스트 진행도 증가
    public void AddProgress(QuestData data, int amount = 1)
    {
        if (data == null) return;
        // 현재 진행 중인 퀘스트 찾기
        var quest = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (quest == null) return;
        // 진행도 증가
        quest.AddProgress(amount);
        // 목표 달성 여부에 따라 completed 저장
        int isCompleted = quest.State == EnumTypes.QUEST.OBJECTIVE_DONE ? 1 : 0;
        SaveQuestState(data.QuestID, quest.CurrentCount, 1, isCompleted, 0);

        OnQuestUpdate?.Invoke();
    }

    // NPC에게 보고하고 최종 처리 함수
    public void CompletedQuest(QuestData data)
    {
        // 현재 진행중인 퀘스트 찾기
        var quest = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (quest == null) return;
        // 완료 상태인지 확인
        if (quest.State != EnumTypes.QUEST.OBJECTIVE_DONE) return;
        // 상태 변경
        quest.SetState(EnumTypes.QUEST.COMPLETED);
        // 실행중인 퀘스트 -> 완료된 퀘스트로 이동
        activeQuest.Remove(quest);
        completeQuest.Add(quest);
        // 다음 퀘스트 해금
        UnlockNextQuest(data);

        // completed는 보상이 끝난 상태
        SaveQuestState(data.QuestID, quest.CurrentCount, 1, 1, 1);

        OnQuestUpdate?.Invoke();
        OnQuestCompleted?.Invoke(data);
    }

    // 다음 퀘스트 해금
    private void UnlockNextQuest(QuestData data)
    {
        if (data == null) return;
        if (data.NextQuestID == 0) return;
        // 다음 퀘스트 id 찾고 추가 
        if(questDict.ContainsKey(data.NextQuestID))
        {
            availableQuestIDs.Add(data.NextQuestID);
        }
    }
    // 현재 진행중인 퀘스트 확인후, QuestData 반환 
    public QuestData GetCurrentQuestData()
    {
        QuestInstance active = GetActiveInstance();
        return active != null ? active.Data : null;
    }
    // 진행 중이거나 목표 도달 상태인 퀘스트 반환 
    public QuestInstance GetActiveInstance()
    {
        return activeQuest.Find(q=>q.State == EnumTypes.QUEST.IN_PROGRESS ||
                                q.State == EnumTypes.QUEST.OBJECTIVE_DONE);
    }

    // DB에서 퀘스트 상태를 불러와 복원
    public void LoadQuestFromDB()
    {
        if (GameManager.Instance == null || GameManager.Instance.DataManager == null || GameManager.Instance.SQLiteManager == null) return;
        var gm = GameManager.Instance;
        int playerId = gm.DataManager.PlayerData.Id;
        var sqlite = gm.SQLiteManager;

        questStateMap.Clear();
        activeQuest.Clear();
        completeQuest.Clear();
        availableQuestIDs.Clear();
        availableQuestIDs.Add(firstQuestID);

        List<PlayerQuestSaveData> savedQuest = sqlite.LoadPlayerQuest(playerId);

        foreach(var saved in savedQuest)
        {
            // 메모리 캐시 저장
            questStateMap[saved.QuestId] = saved;
            if (!questDict.TryGetValue(saved.QuestId, out QuestData questData)) continue;

            // 저장된 데이터를 기반으로 인스턴스 복원
            QuestInstance instance = new QuestInstance(questData);

            // 진행도 복원
            if(saved.CurrentCount > 0)
            {
                instance.SetProgress(saved.CurrentCount);
            }
            // saved의 상태에 따라 State 변경 
            if(saved.IsRewardClaimed == 1)
            {
                instance.SetState(EnumTypes.QUEST.COMPLETED);
                completeQuest.Add(instance);
                UnlockNextQuest(questData);
            }
            else if(saved.IsAccepted == 1)
            {
                if (saved.IsCompleted == 1)
                {
                    instance.SetState(EnumTypes.QUEST.OBJECTIVE_DONE);
                }
                else instance.SetState(EnumTypes.QUEST.IN_PROGRESS);

                activeQuest.Add(instance);
            }
        }
        OnQuestUpdate?.Invoke();
    }
    // 퀘스트 상태 DB 저장
    private void SaveQuestState(int questId, int currentCount, int isAccepted, int isCompleted, int reward)
    {
        if (GameManager.Instance == null || GameManager.Instance.DataManager == null || GameManager.Instance.SQLiteManager == null) return;
        int playerId = GameManager.Instance.DataManager.PlayerData.Id;
        // 저장용 데이터 
        PlayerQuestSaveData data = new PlayerQuestSaveData
        {
            PlayerId = playerId,
            QuestId = questId,
            CurrentCount = currentCount,
            IsAccepted = isAccepted,
            IsCompleted = isCompleted,
            IsRewardClaimed = reward
        };

        questStateMap[questId] = data;
        GameManager.Instance.SQLiteManager.SavePlayerQuest(data);
    }

    public PlayerQuestSaveData GetQuestSaveData(int questId)
    {
        if (questStateMap.TryGetValue(questId, out PlayerQuestSaveData data)) return data;

        return null;
    }
}
