# Apocalypse - Unity 2D 액션/플랫포머 게임

Unity 6000.1.2f1로 구축된 정교한 2D 액션/플랫포머 게임으로, 강력한 이벤트 기반 아키텍처, 챕터 기반 진행 시스템, 그리고 포괄적인 저장/로드 기능을 특징으로 합니다.

## 목차

- [시스템 개요](#시스템-개요)
- [핵심 아키텍처](#핵심-아키텍처)
- [주요 시스템](#주요-시스템)
- [프로젝트 구조](#프로젝트-구조)
- [개발 환경 설정](#개발-환경-설정)
- [빌드 지침](#빌드-지침)
- [아키텍처 패턴](#아키텍처-패턴)
- [애셋 관리](#애셋-관리)
- [성능 고려사항](#성능-고려사항)

## 시스템 개요

Apocalypse는 느슨한 결합과 유지보수성을 촉진하는 모듈식, 이벤트 기반 아키텍처를 중심으로 설계되었습니다. 게임의 주요 특징:

- **챕터 기반 진행**: 매끄러운 전환이 가능한 조직화된 스테이지
- **동적 애셋 로딩**: Unity Addressables를 사용한 효율적인 메모리 관리
- **이벤트 기반 게임플레이**: 중앙 집중식 이벤트 시스템을 통한 모든 주요 게임 액션 처리
- **포괄적인 저장 시스템**: 스크린샷 미리보기가 포함된 JSON 기반 지속성
- **비동기/대기 패턴**: UniTask를 사용한 논블로킹 작업
- **모듈식 캐릭터 시스템**: 상태 머신을 갖춘 플러그인 방식의 아바타 타입 (Hero, Heroine)

## 핵심 아키텍처

### 이벤트 시스템 (`Assets/Scripts/Event/`)

애플리케이션의 핵심은 이벤트 기반 아키텍처입니다:

**GameEventManager** (`GameEventManager.cs:6`)
- 모든 게임 이벤트를 관리하는 중앙 싱글톤
- 호환성 검사가 포함된 큐 기반 이벤트 처리
- 비독점 이벤트를 종료할 수 있는 독점 이벤트 지원
- 자동 이벤트 생명주기 관리

**GameEventFactory** (`GameEventFactory.cs`)
- 이벤트 생성을 위한 팩토리 패턴
- 런타임 및 데이터 기반 이벤트 생성 (DTO) 지원
- 자주 사용되는 작업을 위한 공통 이벤트 템플릿

**이벤트 타입:**
- `SceneLoadEvent`: 씬 전환 및 로딩
- `DataSaveEvent`/`DataLoadEvent`: 저장/로드 작업
- `StoryEvent`: 내러티브 진행
- `UIChangeEvent`: 인터페이스 상태 변경
- `StageTransitionEvent`: 레벨 진행
- `ChoiceEvent`: 플레이어 결정 처리

### 애셋 관리 시스템

**Unity Addressables 통합**
- 조직화된 애셋 그룹: Maps, Stage Profile, Story, Systems, UIAssets, Weapons, Enemy Utilities
- 비동기 로딩으로 프레임 드롭 방지
- 자동 메모리 관리 및 언로딩
- 개발을 위한 핫 로딩 가능한 콘텐츠

**StageScene** (`StageScene.cs:7`)
- 이전/현재/다음 스테이지 관리를 통한 동적 스테이지 로딩
- 스냅 포인트를 사용한 매끄러운 스테이지 전환
- 자동 전환을 위한 플레이어 위치 모니터링
- 인접 스테이지만 로드하여 효율적인 메모리 사용

### 저장/로드 시스템 (`DataManager.cs:12`)

**기능:**
- 스크린샷 미리보기가 포함된 18개 저장 슬롯
- Newtonsoft.Json을 사용한 JSON 직렬화
- 플레이 시간 추적 및 계산
- 최근 저장 감지 및 로딩
- 메타데이터가 포함된 타입 안전 직렬화

**저장 데이터 구조:**
```csharp
SaveData(ChapterType chapter, int stage, Texture2D image, 
         PlayerAvatarType avatar, string playTime, string saveTime)
```

## 주요 시스템

### 캐릭터 시스템 (`Assets/Scripts/GamePlay/Character/`)

**플레이어 아키텍처:**
- `PlayerController`: 메인 캐릭터 컨트롤러
- `PlayerAvatarBase`: 다양한 캐릭터 타입을 위한 추상 베이스
- 상체/하체 애니메이션을 위한 상태 머신
- 발사체 관리가 포함된 무기 시스템

**적 시스템:**
- `EnemyController`: AI 기반 적 행동
- 상태 기반 AI: 순찰, 추적, 공격, 피해, 사망
- 지형 체크 및 플레이어 감지
- 모듈식 적 타입 (NormalInfectee, Pigeon 등)

### 스테이지 관리 (`Assets/Scripts/Scene/Stage/`)

**핵심 컴포넌트:**
- `StageManager`: 개별 스테이지 생명주기 제어
- `IStageElement`: 상호작용 가능한 스테이지 오브젝트용 인터페이스
- `ObjectReplacementTile`: 타일맵 타일을 상호작용 오브젝트로 변환
- `SnapPoint`: 스테이지 연결 포인트 정의
- `StageBoundary`: 플레이 가능 영역 경계 정의

**스테이지 요소:**
- `PlayerStart`: 플레이어 스폰 포인트
- `KillZone`: 즉사 지역
- `Ladder`: 복합 구조를 가진 등반 가능한 오브젝트
- `MovingWalk`: 애니메이션 플랫폼
- `BreakablePlatform`: 파괴 가능한 지형

### UI 시스템 (`Assets/Scripts/UI/`)

**컨트롤러 패턴:**
- 모든 UI 화면을 위한 베이스 `UIController` 클래스
- 이벤트 기반 UI 상태 변경
- 오버레이 및 메뉴를 위한 모달 시스템
- 저장/로드 인터페이스를 위한 데이터 바인딩

**주요 UI 컨트롤러:**
- `TitleUIController`: 메인 메뉴
- `SaveUIController`/`LoadUIController`: 저장 관리
- `StoryUIController`: 대화 및 내러티브
- `PauseUIController`: 게임 상태 관리

## 프로젝트 구조

```
Assets/
├── Scripts/                    # 핵심 게임 로직
│   ├── Event/                  # 이벤트 시스템 구현
│   ├── GamePlay/               # 게임플레이 메커닉
│   │   ├── Character/          # 플레이어 및 적 시스템
│   │   ├── Object/             # 상호작용 오브젝트 및 발사체
│   │   └── Pooling/            # 오브젝트 풀링 시스템
│   ├── Scene/                  # 씬 관리
│   │   └── Stage/              # 스테이지별 로직
│   ├── Player/                 # 플레이어 데이터 및 설정
│   ├── UI/                     # 사용자 인터페이스 컨트롤러
│   └── Utility/                # 헬퍼 시스템
├── GameResources/              # 게임 콘텐츠
│   ├── Prefab/                 # 게임 오브젝트 프리팹
│   ├── Stage/                  # 스테이지 정의
│   ├── GameData/               # 구성 애셋
│   └── Animation/              # 캐릭터 애니메이션
└── AddressableAssetsData/      # Addressable 애셋 구성
```

## 개발 환경 설정

### 전제 조건
- Unity 6000.1.2f1 이상
- Visual Studio 2022 또는 JetBrains Rider

### 주요 의존성
- **UniTask**: Unity 코루틴을 대체하는 비동기/대기 기능
- **Newtonsoft.Json**: 저장 데이터를 위한 JSON 직렬화
- **Unity Addressables**: 애셋 관리 및 로딩
- **Unity 2D Feature Set**: 스프라이트, 애니메이션, 타일맵, 픽셀 퍼펙트

### 설정 단계
1. 리포지토리 복제
2. Unity 6000.1.2f1+에서 프로젝트 열기
3. Package Manager를 통해 모든 패키지가 올바르게 가져와졌는지 확인
4. 첫 빌드 전에 Addressables 콘텐츠 빌드 (Window → Asset Management → Addressables → Groups)

## 빌드 지침

### 개발 빌드
1. **Addressables 빌드**: Window → Asset Management → Addressables → Groups → Build → New Build → Default Build Script
2. **플레이어 빌드**: File → Build Settings → Build
3. 적절한 애셋 로딩을 위해 두 단계 모두 완료되었는지 확인

### 중요 사항
- 플레이어 빌드 전에 항상 Addressables를 먼저 빌드
- 다양한 Addressables 구성(Fast Mode vs Packed Mode)으로 빌드 테스트
- 모든 애셋 참조가 Addressables 그룹에 올바르게 할당되었는지 확인

## 아키텍처 패턴

### 이벤트 기반 아키텍처
모든 주요 게임 상태 변경은 이벤트 시스템을 통해 흐릅니다:
```csharp
// 이벤트 제출
GameEventManager.Instance.Submit(GameEventFactory.CreateSceneLoadEvent(SceneType.StageScene));

// 이벤트 처리는 이벤트 호환성 및 생명주기 관리를 통해 자동으로 수행됩니다
```

### 비동기/대기 패턴
무거운 작업은 논블로킹 실행을 위해 UniTask를 사용합니다:
```csharp
public async UniTask AsyncInitializeScene()
{
    await AsyncLoadEssentialStages();
    await AsyncLoadPlayer();
    PlaceStageObjects();
}
```

### 팩토리 패턴
팩토리를 통한 일관된 오브젝트 생성:
- `GameEventFactory`: 이벤트 생성
- `ProjectileFactory`: 발사체 인스턴스화
- 애셋 팩토리를 위한 Addressables 통합

### 옵저버 패턴
이벤트 시스템은 느슨한 결합을 위해 옵저버 패턴을 구현합니다:
- 이벤트는 구독자에게 자동으로 알림
- 컴포넌트는 특정 이벤트 타입에 등록
- 자동 정리로 메모리 누수 방지

### 상태 머신 패턴
캐릭터 행동은 상태 머신을 사용합니다:
- 플레이어 상체/하체 상태 분리
- 적 AI 상태 관리
- UI 화면 상태 전환

## 애셋 관리

### Addressables 조직
- **Maps**: 스테이지 지오메트리 및 배경
- **Stage Profile**: 스테이지 구성 데이터
- **Story**: 내러티브 애셋 및 스크립트
- **Systems**: 핵심 시스템 프리팹
- **UIAssets**: 인터페이스 요소
- **Weapons**: 전투 관련 애셋
- **Enemy Utilities**: AI 및 적 애셋

### 메모리 관리
- 애셋은 게임 상태에 따라 자동으로 로드/언로드
- 스테이지 시스템은 현재 및 인접 스테이지만 메모리에 유지
- UI 애셋은 빠른 접근을 위해 캐시됨
- 발사체는 가비지 컬렉션 스파이크를 방지하기 위해 오브젝트 풀링 사용

## 성능 고려사항

### 최적화 기술
- **오브젝트 풀링**: 발사체 및 이펙트는 GC 압력을 줄이기 위해 풀링 사용
- **비동기 로딩**: 모든 주요 작업은 논블로킹
- **이벤트 큐잉**: 호환성 검사를 통한 효율적인 이벤트 처리
- **Burst 컴파일**: 성능이 중요한 코드에 대해 활성화
- **Addressables**: 메모리 효율적인 애셋 스트리밍

### 모니터링
- 카테고리별 포괄적인 로깅 시스템 (Event, AssetLoad, GameScene 등)
- Unity Profiler 통합을 통한 성능 프로파일링
- 로드된 애셋에 대한 메모리 사용량 추적

## 개발 가이드라인

### 코드 패턴
- 검증을 위해 Unity Assertions를 광범위하게 사용
- UniTask와 함께 비동기/대기 패턴 따르기
- Addressables 핸들에 대한 적절한 해제 및 정리 구현
- 매니저에 대한 싱글톤 패턴 유지 (PlayerManager, DataManager, GameEventManager)

### 이벤트 개발
- `GameEventFactory`를 통해 이벤트 생성
- 직접 생성 및 직렬화를 위한 DTO 기반 생성 지원
- UniTask를 사용하여 비동기 이벤트를 올바르게 처리
- 이벤트 충돌에 대한 적절한 호환성 검사 구현

### 애셋 개발
- 적절한 Addressables 그룹에 게임플레이 애셋 배치
- 일관된 네이밍 사용: 스테이지는 `{ChapterType}_{StageIndex}`
- 이벤트 구성을 ScriptableObjects로 저장
- 스테이지에 필수 컴포넌트 확보: PlayerStart, SnapPoints, StageTransitionTriggers

### 테스트
- Unity Test Framework 사용 가능하지만 주로 Unity Editor를 통한 수동 테스트
- `Assets/GameResources/Stage/Test/`에서 테스트 씬 사용 가능
- 디버깅 및 개발을 위한 포괄적인 로깅

---

이 README는 Apocalypse 게임 아키텍처에 대한 포괄적인 개요를 제공합니다. 구체적인 구현 세부사항은 인라인 코드 문서와 개발별 지침을 위한 CLAUDE.md 파일을 참조하십시오.