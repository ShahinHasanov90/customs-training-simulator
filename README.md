# customs-training-simulator

A Unity-based 3D training simulator designed for customs officers at land border
crossings. The trainee assumes the role of a newly posted inspector on shift,
walks through a virtualised border checkpoint, and is asked to clear (or hold)
containers arriving at the primary inspection lane. Each scenario introduces
subtle fraud patterns that the trainee must discover through careful document
review, seal verification, and physical inspection.

The simulator is intended to complement classroom instruction for customs
academies and risk-management teams. Scenarios are authored as ScriptableObjects
so subject-matter experts can extend the catalogue without writing C#.

## Gameplay loop

1. Briefing screen explains the shift goal and the expected clearance rate.
2. Trainee spawns in the inspection hall. A container arrives on the conveyor.
3. The trainee approaches the container and opens the paperwork pouch to read
   the declaration (HS code, declared value, origin, consignee).
4. The trainee may break the customs seal (scoring penalty if unjustified),
   open the container, sample cargo, and cross-check the goods against the
   declaration.
5. The trainee decides: RELEASE, HOLD FOR REVIEW, or SEIZE. The decision is
   sent to the DeclarationValidator, which compares it against the scenario's
   ground truth and computes the score delta.
6. Repeat for N containers, then a results screen shows accuracy, average
   decision time, and which fraud patterns were missed.

## Scenario catalogue

The starting catalogue ships three representative ScriptableObject scenarios:

- `Scenario_Undervaluation` — textiles declared at well below market value,
  invoice references a shell company in a free-trade zone.
- `Scenario_Misclassification` — electronics declared under a low-duty HS
  chapter that does not match the physical goods.
- `Scenario_ProhibitedGoods` — cosmetics shipment conceals restricted
  precursor chemicals in a false compartment.

Each scenario is a sequence of `ScenarioStep` assets bound to a `ScoringRules`
profile that weights correctness vs. speed vs. procedural compliance.

## Scoring model

A single inspection round produces three sub-scores:

- Correctness (0-100): did the trainee match the ground-truth decision?
- Procedural (0-100): were seals broken only with cause, was the checklist
  completed, was the documentation logged?
- Speed bonus (0-20): faster-than-target decisions earn a small bonus, but
  only when correctness stays above the pass threshold.

The final score is a weighted sum defined per scenario. Failing correctness
always caps the total at 50, regardless of speed.

## Architecture

The project follows a lightweight layered layout under `Assets/_Project/`:

- `Scripts/Core` — `GameManager`, scene loading, a typed `EventBus`, and a
  minimal `ServiceLocator` for cross-cutting services (audio, analytics).
- `Scripts/Player` — first-person `PlayerController` using the new Input
  System, plus an `InteractionRaycaster` and a small `InventoryComponent`.
- `Scripts/Inspection` — the domain model: `Container`, `Seal`, `Document`,
  `InspectionChecklist`, `InspectionSession`, and `DeclarationValidator`.
- `Scripts/Scenario` — scenario authoring via `ScenarioAsset` and
  `ScenarioStep` ScriptableObjects, executed by `ScenarioRunner`.
- `Scripts/UI` — UGUI views bound to the EventBus.
- `ScriptableObjects/Scenarios` — authored scenario assets in YAML form.
- `Tests/EditMode` and `Tests/PlayMode` — NUnit and Unity Test Framework
  suites covering the validator, scoring rules, checklist, and a minimal end
  to end inspection flow.

Communication between systems is push-based: gameplay code raises events on
`EventBus`, views subscribe. This keeps MonoBehaviours thin and makes edit-mode
tests feasible for most domain logic.

## Requirements

- Unity 2022.3 LTS (tested against 2022.3.40f1)
- Universal Render Pipeline
- Input System package (new input stack)
- TextMeshPro (auto-imported)
- Cinemachine (for briefing camera rigs)

Packages are pinned in `Packages/manifest.json`.

## Opening the project

1. Clone the repository.
2. From Unity Hub, click Add, select the cloned folder, and pick 2022.3 LTS.
3. Unity will restore packages and generate `Library/`. First import can take
   several minutes because URP shaders compile on demand.
4. Open the scene `Assets/_Project/Scenes/InspectionHall.unity` (author your
   own scene in this path; the scaffolding assumes it exists).
5. Press Play. Use WASD to walk, mouse to look, E to interact, Tab for the
   checklist, and Esc to pause.

## Running tests

- Window -> General -> Test Runner.
- Switch to EditMode and run `DeclarationValidatorTests`, `ScoringRulesTests`,
  `InspectionChecklistTests`.
- Switch to PlayMode and run `InspectionFlowTests`.

EditMode tests run headless and are safe for CI. PlayMode tests require the
editor runtime.

## Extending

New fraud patterns: add an entry to the `FraudPattern` enum, extend
`DeclarationValidator.Evaluate`, and author a matching `ScenarioAsset` in the
`Assets/_Project/ScriptableObjects/Scenarios` folder. The runner will pick up
any asset placed under that folder at editor time.

## Status

Gameplay scripts, domain model, three sample scenarios, and a small test suite
are in place. Art and audio assets are intentionally out of scope for this
portfolio scaffold.
