# DAY08 - 통합 테스트와 능력단위 평가 준비

## 1. 통합 테스트 개요

- 테스트 일시: 2026-09-29
- 대상 프로젝트: GameDatabase
- 목적: SQLite, C# 구매 프로그램, 트랜잭션 롤백, LiteDB 퀘스트 저장, Unity SQLite 조회 기능을 각각 재실행하여 정상 동작 여부를 확인한다.

이번 테스트에서는 각 기능을 별도로 확인하였다.

- SQLite: Player, Item, Inventory 데이터 관리
- C# 콘솔 프로그램: 아이템 구매 및 트랜잭션 처리
- LiteDB: 고블린 퀘스트 진행 상태 저장 및 조회
- Unity: SQLite Item 데이터 읽기 전용 표시

콘솔 구매 결과와 LiteDB 퀘스트 데이터, Unity 화면은 서로 자동으로 연결되는 기능으로 가정하지 않는다.


## 2. SQLite / C# 정상 구매 테스트

### 테스트 전 상태

- PlayerId: 1
- Gold: 100
- ItemId: 1
- 포션 수량: 0
- 회복 포션 가격: 30 Gold

### 실행 결과

회복 포션 1개 구매를 실행하였다.

결과:

- 구매 전 Gold: 100
- 구매 전 포션 수량: 0
- 구매 후 Gold: 70
- 구매 후 포션 수량: 1
- 출력 메시지: 구매를 완료했습니다.

### 결과

정상 구매 시 골드가 30 차감되고 포션 수량이 1 증가하는 것을 확인하였다.


## 3. 골드 부족 구매 실패 테스트

### 테스트 전 상태

- Gold: 10
- 포션 수량: 0
- 회복 포션 가격: 30 Gold

### 실행 결과

골드가 부족한 상태에서 구매를 시도하였다.

출력 메시지:

`구매를 취소했습니다: 골드가 부족합니다.`

구매 후 상태:

- Gold: 10
- 포션 수량: 0

### 결과

골드가 부족한 경우 구매가 취소되고 기존 데이터가 변경되지 않는 것을 확인하였다.


## 4. 인벤토리 행 없음 / 트랜잭션 롤백 테스트

### 테스트 전 상태

- Gold: 100
- PlayerId 1 / ItemId 1의 Inventory 행 삭제

골드는 충분하지만 포션 인벤토리 행이 없는 상태에서 구매를 실행하였다.

### 실행 결과

출력 메시지:

`구매를 취소했습니다: 포션 인벤토리 행이 없습니다.`

구매 후 상태:

- Gold: 100
- 포션 수량: 0

### 결과

구매 처리 중 골드 차감 이후 인벤토리 갱신에 실패하면 트랜잭션이 Rollback되어 Gold가 다시 100으로 유지되는 것을 확인하였다.

테스트 후 Inventory 행을 다시 생성하여 복구하였다.


## 5. LiteDB 고블린 퀘스트 테스트

LiteDB의 QuestProgress 데이터를 이용하여 고블린 처치 퀘스트를 확인하였다.

### 테스트 내용

퀘스트 진행 상태를 초기화한 후 고블린 처치를 3회 실행하였다.

### 실행 결과

- 처치 수: 3/3
- 완료 여부: True
- 보상 수령 여부: False

### 재실행 확인

프로그램을 종료한 뒤 다시 실행하였다.

재실행 후에도 다음 데이터가 유지되었다.

- 처치 수: 3/3
- 완료 여부: True
- 보상 수령 여부: False

### 결과

LiteDB 파일에 저장된 퀘스트 데이터가 프로그램 종료 후에도 유지되는 것을 확인하였다.


## 6. Unity SQLite 조회 테스트

Unity 프로젝트의 ShopLab 씬에서 SQLite 데이터를 읽어 UI에 표시하였다.

### Unity 프로젝트 경로

`C:\Users\Admin\Desktop\GameDatabase\GameDatabaseUnity`

### Unity에서 사용하는 DB

`GameDatabaseUnity\Assets\GameShop.sqlite`

### 확인 결과

Game 화면에 다음 내용이 정상적으로 표시되었다.

`회복 포션 / 가격: 50`

Play Mode를 종료한 뒤 다시 실행해도 동일하게 표시되었다.

### 결과

Unity에서 GameShop.sqlite의 Item 데이터를 읽어 ItemText에 정상적으로 표시하는 것을 확인하였다.

※ C# 구매 테스트에 사용한 `GameShop.db`와 Unity에서 읽는 `GameShop.sqlite`는 별도의 파일이다.  
현재 C# 테스트용 GameShop.db의 회복 포션 가격은 30이며, Unity 테스트용 GameShop.sqlite에는 가격 50이 저장되어 있다.


## 7. 데이터베이스 파일 경로

### SQLite

`C:\Users\Admin\Desktop\GameDatabase\GameDatabaseLab\GameShop.db`

### LiteDB 퀘스트 DB

`C:\Users\Admin\Desktop\GameDatabase\GameDatabaseLab\QuestProgress.db`

### Unity SQLite 복사본

`C:\Users\Admin\Desktop\GameDatabase\GameDatabaseUnity\Assets\GameShop.sqlite`


## 8. 통합 테스트 결과표

| 번호 | 테스트 | 결과 |
| --- | --- | --- |
| 1 | Gold 100으로 포션 구매 | Gold 70 / 포션 1 확인 |
| 2 | Gold 10으로 포션 구매 | 구매 취소 / Gold 10 / 포션 0 유지 |
| 3 | Inventory 행 없이 구매 | 구매 실패 후 Gold 100으로 Rollback 확인 |
| 4 | 고블린 퀘스트 처치 3회 | 3/3 / 완료 True 확인 |
| 5 | Unity Item 첫 행 조회 | 회복 포션 / 가격 50 표시 확인 |
| 6 | 프로그램 재실행 | LiteDB 퀘스트 데이터 및 Unity 표시 유지 확인 |


## 9. 유지보수 확인 사항

### 정상 구매·실패 구매 결과

- 정상 구매: Gold 100 → 70, 포션 0 → 1
- 골드 부족: Gold 10 유지, 포션 0 유지
- 인벤토리 행 없음: 구매 실패 후 Gold 100 유지

### Unity Item 조회 결과

- 아이템 이름: 회복 포션
- 가격: 50
- ItemText 정상 표시 확인
- Play 재실행 후 동일한 값 표시 확인

### LiteDB 확인 결과

- 고블린 처치 수: 3/3
- 완료 여부: True
- 프로그램 재실행 후 데이터 유지 확인

### 재현하지 못한 오류

현재 통합 테스트에서 해결되지 않은 오류는 없음.

오류가 발생할 경우 다음 순서로 확인한다.

1. 실행 중인 DB 파일 경로 확인
2. DB Browser에서 실제 저장값 확인
3. 변경사항 저장 여부 확인
4. C# 프로그램의 연결 문자열 확인
5. LiteDB 파일 경로 확인
6. Unity의 Database Asset 연결 확인
7. Unity Console 오류 확인


## 10. 최종 자기 점검

- [x] 관계형 DB와 문서형 NoSQL DB의 역할을 구분하였다.
- [x] SQLite에서 Player, Item, Inventory 데이터를 확인하였다.
- [x] 정상 구매를 실행하였다.
- [x] 골드 부족 구매 실패를 확인하였다.
- [x] 트랜잭션 Rollback을 확인하였다.
- [x] LiteDB 퀘스트 문서 생성·조회·수정을 확인하였다.
- [x] LiteDB 데이터의 재실행 유지 여부를 확인하였다.
- [x] Unity에서 SQLite Item 데이터를 조회하였다.
- [x] Unity ItemText와 DB 데이터를 비교하였다.
- [x] 통합 테스트 결과를 문서로 기록하였다.


## 11. 최종 결과

SQLite 구매 처리, 실패 처리와 트랜잭션 Rollback, LiteDB 퀘스트 진행 저장, Unity의 SQLite 읽기 전용 조회를 각각 정상적으로 확인하였다.

각 데이터베이스 파일과 실행 결과를 통해 프로그램을 종료한 뒤에도 데이터가 유지되는 것을 확인하였다.

Day08 통합 테스트 및 능력단위 평가 준비를 완료하였다.