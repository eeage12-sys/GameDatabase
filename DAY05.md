# DAY05 - C# 트랜잭션과 롤백

## 프로그램 정보

프로그램 이름: GameShop Database Lab

DB 파일 위치:
GameDatabaseLab/GameShop.db

실행 방법:
dotnet run

## 사용 테이블

- Player
- Item
- Inventory

## 구현 내용

- C#에서 SqliteTransaction 사용
- 골드 차감과 아이템 수량 증가를 하나의 트랜잭션으로 처리
- 정상 처리 시 Commit()
- 오류 발생 시 Rollback()
- 인벤토리 데이터에 UPSERT 적용
- 구매 수량 입력 기능 구현
- 가격 × 수량으로 총 구매 가격 계산

## 테스트 결과

- 정상 구매 성공
- 골드 부족 시 구매 취소 확인
- 골드 부족 시 아이템 수량이 증가하지 않음
- 두 번째 DB 명령 실패 시 골드 차감도 롤백됨
- UPSERT 정상 동작 확인
- 구매 수량 입력 정상 동작 확인
- 수량이 0 이하일 경우 구매 거부 확인

## 알려진 제한

로컬 단일 사용자 학습용 데이터베이스이며
온라인 게임 서버용 데이터베이스는 아니다.