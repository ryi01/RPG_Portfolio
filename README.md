# 3D RPG Portfolio

퀘스트 수락부터 던전 탐험, 보스 전투, 보상 획득까지 이어지는 구조의 3D RPG 프로젝트입니다.  
절차적 던전 생성, FSM 기반 전투 시스템, 인벤토리/상점 시스템, SQLite 연동을 중심으로 구현했습니다.

---

## 프로젝트 개요

- **장르**: 3D RPG
- **개발 인원**: 1인
- **개발 환경**: Unity, C#
- **주요 기능**
  - 절차적 던전 생성
  - A* 기반 경로 탐색
  - 퀘스트 시스템
  - Enemy / Boss FSM
  - 인벤토리 및 상점 시스템
  - SQLite 데이터 저장

---

## 게임 진행 흐름

NPC와 상호작용하여 퀘스트를 수락한 뒤, 절차적으로 생성된 던전에 입장합니다.  
던전에서 전투와 탐험을 진행하고 보스를 처치하면 마을로 복귀하여 보상을 수령하고 다음 퀘스트를 진행할 수 있습니다.

<img width="1680" height="980" alt="Untitled" src="https://github.com/user-attachments/assets/00b2c900-3da4-4358-9bea-19d08f5ab819" />

---

## 주요 시스템

### 1. 퀘스트 시스템
QuestManager를 중심으로 퀘스트 상태를 관리하도록 구성했습니다.  
NPC 상호작용, 현재 퀘스트 상태 확인, 완료 조건 판정, 보상 처리, 다음 퀘스트 활성화까지 하나의 흐름으로 연결되도록 설계했습니다.

<img width="1680" height="980" alt="Untitled (1)" src="https://github.com/user-attachments/assets/c614381f-af8b-4b1e-b365-89ef469d4b52" />

**핵심 포인트**
- 퀘스트 수락 / 진행 / 완료 상태 관리
- 완료 조건 충족 여부 확인
- 보상 지급 및 다음 퀘스트 활성화
- NPC와의 상호작용을 통한 흐름 연결

---

### 2. Enemy / Boss FSM
적과 보스의 행동을 상태 기반으로 관리했습니다.  
일반 적은 Idle, Detect, Attack, Return, Damage, Stun, Death 흐름으로 구성했고,  
보스는 여기에 패턴 전환과 페이즈 변경 상태를 추가해 전투 흐름을 확장했습니다.

<img width="1680" height="980" alt="Untitled (2)" src="https://github.com/user-attachments/assets/1a7faf72-745b-4f76-9626-240dfc83cbe1" />

**핵심 포인트**
- 상태 전환 기반 행동 제어
- 일반 적과 보스 FSM 분리
- 보스 패턴 전환 및 페이즈 변화 처리
- 상태별 애니메이션 및 전투 로직 연결

---

### 3. 인벤토리 시스템
아이템 데이터, 인벤토리 UI, 아이템 박스, SQLite 저장 구조를 연결해 관리했습니다.  
아이템 획득부터 사용, 저장까지 흐름이 이어지도록 구성했습니다.

<img width="1680" height="980" alt="Untitled (3)" src="https://github.com/user-attachments/assets/a7eba988-5151-465a-817b-31c14891175a" />

**핵심 포인트**
- 아이템 데이터 기반 관리
- 인벤토리 UI와 데이터 연결
- 아이템 박스를 통한 획득 처리
- SQLite를 통한 인벤토리 데이터 저장

---

## 핵심 기술

### 절차적 던전 생성
방을 생성한 뒤 Delaunay Triangulation으로 후보 연결을 만들고,  
MST를 이용해 핵심 경로를 구성한 다음 일부 추가 간선을 연결해 자연스러운 던전 구조를 만들었습니다.

<img width="1680" height="857" alt="Untitled (4)" src="https://github.com/user-attachments/assets/0dbc7f71-5c5b-4ab9-ae00-ff8d1e545401" />

**적용 내용**
- 방 랜덤 생성
- Delaunay Triangulation
- MST 기반 핵심 경로 구성
- 추가 경로 연결로 탐험 다양성 확보

---

### A* Pathfinding
그리드 기반 A* 알고리즘을 사용해 플레이어가 장애물을 피해 탐험할 수 있도록 만들었습니다.

<img width="1680" height="980" alt="Untitled (5)" src="https://github.com/user-attachments/assets/d022a091-0dee-4d12-a061-91e5e99b1075" />

**적용 내용**
- 그리드 노드 기반 탐색
- 최단 경로 계산
- 장애물 회피 처리
- 전투 상황에서의 추적 이동 적용

---

### SQLite 데이터 관리
플레이어, 인벤토리, 퀘스트 데이터를 SQLite로 저장하여  
게임 진행 상태를 구조적으로 관리할 수 있도록 구성했습니다.

<img width="1018" height="694" alt="포트폴리오 (1)" src="https://github.com/user-attachments/assets/8d1306f1-6bc9-4977-9d65-c7a9457d795b" />

**적용 내용**
- 플레이어 데이터 저장
- 인벤토리 정보 저장
- 퀘스트 진행 상태 저장
- 테이블 기반 데이터 관리

---

## 담당 구현

- 퀘스트 시스템 설계 및 구현
- 절차적 던전 생성 로직 구현
- Enemy / Boss FSM 구현
- 인벤토리 및 상점 시스템 구현
- SQLite 연동 및 데이터 관리
- 전투 및 상호작용 시스템 구현

---

## 기술 스택

- **Engine**: Unity
- **Language**: C#
- **Database**: SQLite
- **Algorithm**: A*, Delaunay Triangulation, MST
- **Tools**: Git, Figma, DB Browser for SQLite

---
## 회고

이 프로젝트를 통해 단순 기능 구현을 넘어,  
여러 시스템이 서로 연결되는 구조를 설계하고 관리하는 경험을 할 수 있었습니다.  
특히 퀘스트, 전투, 인벤토리, 데이터 저장을 각각 분리하면서도 하나의 게임 흐름으로 연결하는 과정에서  
시스템 중심으로 프로젝트를 바라보는 경험을 쌓을 수 있었습니다.
