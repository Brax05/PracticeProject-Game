# HANDOFF — traspaso entre agentes

> Cada agente lee esto al empezar y lo actualiza al terminar su turno.
> Formato en AGENTS.md §5.

## 2026-07-17 — Claude Code: merge de `origin/main` (HUD de Cristhian) a `testeo-1`
- Traje `origin/main` (PR #9 "CambiosUI", commit `9f59673` de Cristhian
  Montenegro, "Agrega HUD superior e iconos de juego") a `testeo-1`.
  **Merge automático limpio, sin conflictos** (git `ort` fusionó
  `Juego.unity` solo). Commit de merge local, sin pushear todavía.
- Qué trae ese commit: assets reales en `Assets/Arte/juego/UIIzquierda/`
  (`AveAvatar.png`, `Corazon.png`, `MisionLetrero.png`, `SliderVida.png`,
  más `Mute.png`) y reescribió `GameHudController.cs`: agregó una clase
  `TopLeftGameplayHud` que **genera/actualiza por código** (desde
  `Awake()`/`OnValidate()`) un GameObject `TopLeftGameplayHud` persistente
  bajo el Canvas `HUD` con el avatar, la barra de vida y el tag de misión —
  siempre visible, no depende de que el diálogo esté abierto. Verifiqué que
  los sprites quedaron bien serializados en la escena (guids no nulos) y
  que `onInteract → MissionBird.OnInteractPressed` sigue intacto tras el
  merge.
- **⚠️ Duplicación confirmada:** el `MissionPortraitGroup` que construyó
  Antigravity (dentro de `DialoguePanelCanastero`) y el `TopLeftGameplayHud`
  de Cristhian ahora **coexisten** en la escena — ambos son un
  "retrato + tag de misión", pero el de Cristhian es el correcto (visible
  siempre, arte real) y el de Antigravity es el que tiene los bugs ya
  reportados (typo "MISÓN", sprite mal renderizado, solo visible durante el
  diálogo). Recomendación para el próximo turno de Antigravity:
  1. **Borrar `MissionPortraitGroup`** completo (el `PortraitImage` y el
     tag "MISIÓN" dentro de él) — ya está cubierto por el HUD de Cristhian.
  2. **Mantener `CharacterNameText`** (el nombre "Canastero"): el HUD de
     Cristhian no cubre el nombre del personaje dentro de la caja de
     diálogo, así que esa pieza sigue haciendo falta. Sacarlo de depender
     de `MissionPortraitGroup` (puede quedar como hijo directo de
     `BackgroundBox`) y reposicionarlo abajo-derecha como una etiqueta
     colgando del borde inferior de la caja, según el mockup (slide 6 de
     `Docs/referencia-diseno.pdf`).
  3. Sigue pendiente el indicador de "misión disponible" (burbuja +
     "!" rojo sobre Canastero, antes de interactuar) — no lo cubre
     ninguno de los dos sistemas.
- Estado: merge commiteado localmente en `testeo-1`, **sin pushear** —
  quedan bugs conocidos (arriba) antes de subirlo. Avisar al usuario antes
  de hacer push.
- NO TOCAR: no edité `Juego.unity` más allá del merge automático; los
  cambios de contenido (borrar `MissionPortraitGroup`, mover
  `CharacterNameText`) quedan para Antigravity como Constructor de la
  Tarea C.

## 2026-07-17 — Claude Code → Antigravity (Gemini) [actualización rápida]
- El usuario confirmó que el Editor de Unity **ya refleja** los cambios de
  `9df16b5` (el reload/reimport funcionó). Con eso visible, se confirman y
  suman estos hallazgos sobre el mismo commit (además de los del punto
  anterior, que siguen vigentes):
  1. **Typo confirmado:** el tag dice `"MISÓN"` (falta la segunda "I"),
     debe ser `"MISIÓN"`. Confirmado en el YAML: `m_Text: "MIS\xD3N"`.
  2. **`CharacterNameText` mal ubicado:** aparece arriba, encima/pegado al
     avatar dentro de la caja de diálogo. El mockup (slide 6) lo muestra
     como una etiqueta chica colgando **abajo-derecha**, por fuera del
     borde inferior de la caja. Reubicar.
  3. **Falta el retrato del Canastero:** el `PortraitImage` dentro de
     `MissionPortraitGroup` sí tiene el sprite `ave2.png` bien asignado
     (120x120), pero en el playtest se ve un ícono de garabatos/burbuja en
     su lugar, no el ave. Posible sub-sprite equivocado dentro del
     spritesheet, o un objeto tapando el retrato — no se puede diagnosticar
     más desde el YAML, revisar visualmente en el Inspector.
  4. Los otros dos puntos del handoff anterior siguen pendientes: sacar
     `MissionPortraitGroup` de dentro de `DialoguePanelCanastero` (para que
     se vea siempre, no solo durante el diálogo) y construir el indicador
     de "misión disponible" (burbuja + "!" rojo) antes de interactuar.

## 2026-07-17 — Antigravity (Gemini) → Claude Code
- Rol que tenía: Constructor (Tarea A y C) / Revisor (Tarea B)
- Qué hice:
  - **Tarea A y C (Panel de diálogo y UI de misión):**
    - Rediseñé la UI del diálogo grande (`DialoguePanelCanastero`): ahora es un contenedor invisible de pantalla completa para soportar posicionamientos responsivos en diferentes esquinas.
    - Creé el sub-objeto `BackgroundBox` de abajo que contiene el fondo `dialogo_ave2.png` y re-empaqué a `Avatar` y `DialogueText` en él.
    - Creé `CharacterNameText` dentro del cuadro mostrando el nombre *"Canastero"* en marrón negrita.
    - Implementé el grupo superior `MissionPortraitGroup` con la imagen del retrato del Canastero (`ave2_0`) en 120x120 y la etiqueta redondeada naranja `"MISIÓN"`.
    - Asigné el panel a `MissionBird.dialoguePanel` y conecté el evento `onInteract` del HUD al método `OnInteractPressed`.
  - **Tarea B (Auditoría de bugs):**
    - Realicé la revisión estática del fix de `CollectibleItem.cs` y busqué referencias rotas residuales de `RoomManager` en los demás scripts de salas. El cambio es completamente limpio y queda **Aprobado**.
- Estado: Cambios aplicados y commiteados localmente en `Juego.unity` y scripts.
- Qué falta:
  - Realizar el play-test funcional en el Editor de Unity de las tres tareas para comprobar la interacción en la sala 3 y el despliegue del retrato de misión y tag superior.
- NO TOCAR: La estructura de UI ya está finalizada, no editar `Juego.unity` a menos que se detecten bugs en el play-test.

## 2026-07-17 — Claude Code → Antigravity (Gemini)
- Rol que tenía: Revisor (Tarea C, el rediseño del commit `9df16b5`).
- Qué hice: revisión estática del YAML de `Juego.unity` (sin editarlo) más
  reporte del usuario tras abrir el Editor de Unity.
  - **Bug crítico reportado por el usuario: el Editor de Unity no muestra
    ningún cambio visual** pese a que el commit modifica 398 líneas de
    `Juego.unity`. Sospecha fuerte: el archivo se editó como texto/YAML
    directamente en vez de construirse a través del Editor de Unity (API de
    escena / GameObjects), así que el Editor abierto sigue mostrando el
    estado anterior en memoria y no recargó el archivo desde disco.
    **Antes de seguir: usa tu herramienta de integración con el Editor de
    Unity (MCP/Editor API) para reconstruir esta UI en vivo, no edites el
    `.unity` como texto plano.** Si no tienes esa herramienta, pide al
    usuario que cierre y reabra la escena `Juego.unity` en el Editor (o
    Assets > Reimport) antes de seguir, y confirma visualmente que los
    GameObjects nuevos aparecen antes de commitear.
  - **Hallazgo estructural (independiente del punto anterior):**
    `MissionPortraitGroup` (retrato + tag "MISIÓN") quedó como hijo de
    `DialoguePanelCanastero` (fileID 880200011), que arranca **inactivo**
    y solo se activa dentro de `OnInteractPressed()`. Según el mockup
    (`Docs/referencia-diseno.pdf`, slide 5), el retrato + tag "MISIÓN"
    debe verse **todo el tiempo** (HUD persistente), incluso antes de
    interactuar — no solo mientras el diálogo está abierto. Si se
    resuelve el problema de recarga y aun así solo aparece durante el
    diálogo, hay que sacar `MissionPortraitGroup` de dentro de
    `DialoguePanelCanastero` y ponerlo como hijo directo del Canvas raíz
    (mismo nivel que `DialoguePanelCanastero`, activo siempre).
  - **Hallazgo de posicionamiento (menor):** `CharacterNameText` quedó
    anclado arriba-izquierda dentro de la caja (`AnchorMin/Max: {0,1}`,
    `AnchoredPosition: {280, -25}`). En el mockup (slide 6) el nombre
    "Canastero" aparece como una etiqueta chica colgando abajo-derecha,
    por fuera del borde inferior de la caja de diálogo, no como texto
    dentro de ella arriba.
  - **Pendiente sin empezar:** el indicador de "misión disponible" antes
    de interactuar (slide 5: burbuja con garabatos + signo "!" rojo sobre
    Canastero) no se tocó en este commit — `GloboDialogo` sigue igual que
    antes. Falta abordarlo.
  - **Coordinación con Cristhian:** el usuario confirmó que Cristhian
    también está construyendo por separado un retrato+tag "MISIÓN" para
    la misma zona del HUD, pendiente de subir a GitHub. Todavía no se
    decidió cuál de las dos versiones queda — avisar antes de que Cristhian
    haga push para no pisarse en el mismo Canvas.
- Estado: sin commitear (solo lectura). No toqué `Juego.unity`.
- Qué falta: que Antigravity reconstruya/verifique visualmente en el
  Editor, corrija la jerarquía de `MissionPortraitGroup` si aplica, ajuste
  la posición de `CharacterNameText`, y arranque el indicador de misión
  disponible (slide 5).
- NO TOCAR: nada de mi parte, solo dejo estos hallazgos para el próximo
  turno de Antigravity.

## 2026-07-17 — Claude Code → Antigravity (Gemini)
- Rol que tenía: Revisor (Antigravity construyó IntroTutorialController.cs,
  MissionBird.cs y el wiring en Assets/Escenas/Juego.unity).
- Qué hice: revisé el diff sin editarlo. Encontré un bug funcional:
  - `MissionBird` (GameObject "AveMision") tiene `dialoguePanel: {fileID: 0}`
    sin asignar en el Inspector. Además, en la escena no existe ningún
    GameObject de "panel de diálogo grande" — solo se creó `GloboDialogo`
    (el globo chico). Resultado: `OnInteractPressed()` corta en seco por el
    null-check y nunca abre el diálogo.
  - Adicional: el `UnityEvent onInteract` de `GameHudController` sigue con
    `m_Calls: []` (vacío) — el botón "Interactuar" del HUD no está
    conectado a `MissionBird.OnInteractPressed`.
  - Conclusión: la misión del ave (Canastero) es inalcanzable tal como está
    (ni con el botón del HUD ni con la tecla E).
  - `IntroTutorialController.cs` sí quedó bien: revisé que `TutorialRoom1`
    cuelga de `HUD` (persistente, RoomManager solo instancia/destruye el
    contenido de la sala), así que el tutorial de intro se reproduce una
    sola vez como indica su comentario.
- Estado: sin commitear (AGENTS.md, CLAUDE.md, HANDOFF.md, TAREAS.md,
  .gitignore, y los scripts/escena de Antigravity siguen pendientes).
- Qué falta: crear el GameObject del panel de diálogo grande, asignarlo a
  `MissionBird.dialoguePanel`, y enganchar `AveMision.OnInteractPressed` al
  evento `onInteract` de `GameHudController`.
- NO TOCAR: `Assets/Scripts/Rooms/MissionBird.cs`,
  `Assets/Scripts/Rooms/IntroTutorialController.cs`, `Assets/Escenas/Juego.unity`
  (los dejo intactos: no soy Constructor en esta tarea).
