# 3D RPG System Portfolio

퀘스트 → 던전 → 보스 전투 → 보상으로 이어지는 구조의 3D RPG입니다.  
시스템 설계와 구조 구현에 집중했습니다.

---

## 프로젝트 개요

- **장르**: 3D RPG  
- **인원**: 1인 개발  
- **환경**: Unity / C#  

**핵심 구현**
- 절차적 던전 생성 (Delaunay + MST)
- Enemy / Boss FSM
- 퀘스트 시스템
- 인벤토리 및 상점 시스템
- SQLite 데이터 관리

---

## 게임 흐름
<img width="1680" height="980" alt="Untitled" src="https://github.com/user-attachments/assets/00b2c900-3da4-4358-9bea-19d08f5ab819" />

NPC → 퀘스트 수락 → 던전 입장 → 보스 처치 → 보상 → 다음 퀘스트 진행

---

## 주요 시스템

### 퀘스트 시스템

QuestManager를 중심으로 퀘스트 상태를 관리하도록 구성했습니다.  
NPC 상호작용, 현재 퀘스트 상태 확인, 완료 조건 판정, 보상 처리, 다음 퀘스트 활성화까지 하나의 흐름으로 연결되도록 설계했습니다.

<img width="1680" height="980" alt="Untitled (1)" src="https://github.com/user-attachments/assets/c614381f-af8b-4b1e-b365-89ef469d4b52" />

- QuestManager 중심 상태 관리
- 퀘스트 수락 / 진행 / 완료 흐름 분리
- 완료 조건 체크 후 보상 지급 및 다음 퀘스트 활성화

---

### Enemy / Boss FSM

적과 보스의 행동을 상태 기반으로 관리했습니다.  
일반 적은 Idle, Detect, Attack, Return, Damage, Stun, Death 흐름으로 구성했고,  
보스는 여기에 패턴 전환과 페이즈 변경 상태를 추가해 전투 흐름을 확장했습니다.

<img width="1680" height="980" alt="Untitled (2)" src="https://github.com/user-attachments/assets/1a7faf72-745b-4f76-9626-240dfc83cbe1" />

- 상태 기반 행동 제어 (Idle / Detect / Attack / Death 등)
- 일반 적과 보스 FSM 분리
- 보스 페이즈 및 패턴 전환 구현

---

### 인벤토리 시스템
아이템 데이터, 인벤토리 UI, 아이템 박스, SQLite 저장 구조를 연결해 관리했습니다.  
아이템 획득부터 사용, 저장까지 흐름이 이어지도록 구성했습니다.

<img width="1680" height="980" alt="Untitled (3)" src="https://github.com/user-attachments/assets/a7eba988-5151-465a-817b-31c14891175a" />

- 아이템 데이터 기반 관리 (ScriptableObject)
- UI와 데이터 분리 구조
- 아이템 획득 / 사용 / 저장 흐름 구성
- SQLite 연동

## 핵심 기술

### 절차적 던전 생성
- Delaunay Triangulation으로 방 연결
- MST로 핵심 경로 구성
- 일부 간선 추가로 자연스러운 구조 생성
  
<img width="1680" height="857" alt="Untitled (4)" src="https://github.com/user-attachments/assets/0dbc7f71-5c5b-4ab9-ae00-ff8d1e545401" />

---

### A* Pathfinding
- Grid 기반 경로 탐색
- 최단 경로 계산 및 장애물 회피

<img width="1680" height="980" alt="Untitled (5)" src="https://github.com/user-attachments/assets/d022a091-0dee-4d12-a061-91e5e99b1075" />

---

### SQLite
- 플레이어 / 인벤토리 / 퀘스트 데이터 저장
- 테이블 기반 데이터 관리

<img width="1018" height="694" alt="포트폴리오 (1)" src="https://github.com/user-attachments/assets/8d1306f1-6bc9-4977-9d65-c7a9457d795b" />

---

## 담당 구현

- 절차적 던전 생성 시스템
- Enemy / Boss FSM
- 퀘스트 시스템
- 인벤토리 및 상점 시스템
- SQLite 연동
- 플레이어 전투 및 상호작용

---

## 기술 스택

- Unity
- C#
- SQLite
- A*
- Delaunay Triangulation / MST

---

## 회고
이 프로젝트를 통해 단순 기능 구현을 넘어,  
여러 시스템이 서로 연결되는 구조를 설계하고 관리하는 경험을 할 수 있었습니다.  
특히 퀘스트, 전투, 인벤토리, 데이터 저장을 각각 분리하면서도 하나의 게임 흐름으로 연결하는 과정에서  
시스템 중심으로 프로젝트를 바라보는 경험을 쌓을 수 있었습니다.
