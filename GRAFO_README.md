# Prototipo del cubo — Grafo de rooms

Sistema de grafo libre (no direcciones fijas) para probar el recorrido
del boceto antes de tener el arte final. El "personaje" es un cubo
que se mueve libremente dentro de una escena de prueba plana, y al
tocar un trigger de salida salta a la posición del siguiente nodo.

## Setup automático (recomendado)

Los scripts ya están en `Assets/Scripts/Rooms/`. En el menú de Unity,
`Game > Build Graph Prototype Scene (Cap 1)` crea (o actualiza) los 9
`RoomNode` en `Assets/Rooms/` con las conexiones de la tabla de abajo,
arma la escena de prueba completa (`GraphManager`, cubo jugador, un
`RoomExitPoint` por cada dirección de cada conexión) y la guarda en
`Assets/Scenes/GraphPrototype.unity`. Es re-ejecutable: si vuelves a
correrlo, reutiliza los RoomNode existentes y regenera la escena.

Los pasos 1-3 de abajo quedan como referencia de qué hace el botón por
dentro, y para armar manualmente si cambian las conexiones o quieres
ajustar posiciones a mano.

## 1. Crear los RoomNode

`Assets > Create > Game > RoomNode` — crea 9, uno por cada número del
boceto: `RoomNode_1` ... `RoomNode_9`.

En cada uno:
- `roomId`: "1", "2", "3"... (según el boceto)
- `testWorldPosition`: dónde quieres que aparezca el cubo en la escena de prueba para esa room (puedes usar posiciones separadas, ej. cada 10 unidades en X, para probar sin overlap; no necesitan calzar con el boceto real todavía)

## 2. Conectar el grafo

Según lo que definiste, arrastra estas conexiones en el campo `connections` de cada RoomNode (recuerda que es bidireccional — si conectas 1→2, también agrega 1 en la lista de conexiones de 2):

| Room | Conectada con |
|------|----------------|
| 1 | 2 |
| 2 | 1, 3, 5 |
| 3 | 2 |
| 5 | 2, 6, 4 |
| 6 | 5 |
| 4 | 5, 7 |
| 7 | 4, 8 |
| 8 | 7, 9 |
| 9 | 8 |

## 3. Escena de prueba

1. Crea un GameObject vacío `GraphManager` con el componente `RoomGraphManager`.
   - `startingNode` = RoomNode_1
   - `player` = el Transform del cubo (siguiente paso)
2. Crea un Cube 3D o un Sprite cuadrado 2D, agrégale:
   - `Rigidbody2D` (Body Type: Dynamic, Gravity Scale 0)
   - `BoxCollider2D` (NO trigger — este es el collider físico del jugador)
   - `CubePlayerController`
   - Tag `Player`
3. Por cada conexión del grafo, crea un GameObject con `BoxCollider2D` (trigger) + `RoomExitPoint`, posicionado cerca de donde el cubo esté parado en esa room, con el `targetNode` correspondiente asignado. Necesitas un trigger físico por cada dirección de viaje que quieras habilitar en esa posición (ej: en la zona de la room 2 pon un trigger hacia 3 y otro hacia 5).

## 4. Probar

Mueve el cubo con flechas/WASD. Al tocar un trigger, `RoomGraphManager` valida que la conexión exista en el grafo y teletransporta el cubo a `testWorldPosition` del nodo destino. Si intentas ir a una room no conectada (por mal armado del trigger), verás un warning en consola y el cubo no se mueve — así detectas errores de conexión antes de pasar al arte final.

## Siguiente paso (cuando haya arte)

Este sistema de grafo (`RoomNode`) es el equivalente conceptual al
`RoomData` con direcciones fijas del paquete anterior, pero soporta
salidas irregulares como las del boceto. Cuando la diseñadora entregue
las ilustraciones finales de cada celda, se puede fusionar con el
sistema anterior: cada `RoomNode` pasa a tener también su
`roomPrefab` con fondo + Entry points reales, y `RoomExitPoint` en vez
de teletransportar, dispara la misma transición de `RoomManager`
(instanciar prefab + posicionar en el punto de entrada correspondiente).

## Escena real con el primer arte entregado

Con el primer lote de arte (`Assets/assets juego/assets juego aves/`)
ya existe una versión "de verdad" de este mismo grafo de 9 rooms, en
paralelo al prototipo del cubo (que se mantiene intacto como
referencia). Ver `Assets/Editor/BuildGameScene.cs` y el menú
`Game > Build Game Scene (Cap 1)`.

Diferencias con el prototipo:

- El jugador es el Yal austral real (`ave.png` + `BirdPlayerController`,
  namespace `TravesiaACasa.Rooms`) en vez del cubo — mismo esquema de
  Rigidbody2D/BoxCollider2D, así que `RoomExitPoint` no cambió nada.
- Cada room tiene fondo ilustrado, alternando entre `fondo  juego.png`
  y `fondo inicio.png` (los dos únicos fondos "room completa" que hay
  todavía — no hay una ilustración distinta por celda), decoración
  dispersa (`arbusto 1.png`, `nido.png`) y el marco de hojas
  (`hojas.png`) en primer plano.
- La cámara no sigue al jugador en tiempo real: `CameraRoomFollower`
  la deja fija en el centro de la room actual y salta en seco al
  cambiar de nodo (el "sin scroll continuo" que pedía el diseño
  original).
- Usa su **propio** set de `RoomNode` en `Assets/Rooms/Bird/` (mismas
  conexiones 1-9 de la tabla de arriba, pero con posiciones a mayor
  escala para que quepa el fondo panorámico). No toca
  `Assets/Rooms/RoomNode_*.asset`, así que volver a correr
  `Build Graph Prototype Scene (Cap 1)` sigue funcionando igual.
- `Game > Build All Scenes (Menu + Game)` corre este build y el del
  menú (`Assets/Editor/BuildMenuScene.cs`) juntos, y deja
  `MainMenu.unity` y `Game.unity` cargadas en Build Settings.

Pendiente real para cuando existan ilustraciones por celda: reemplazar
el fondo compartido de `BuildRoomVisuals` por el `roomPrefab` propio de
cada `RoomData`, como ya se preveía arriba.

## HUD del juego + Configuración en juego

Al correr `Game > Build Game Scene (Cap 1)` la escena ahora incluye:

- **EventSystem** (antes faltaba: ningún botón del HUD respondía).
- **D-pad** abajo-izquierda con `flecha.png` rotada. Cada flecha usa
  `HudMoveButton` (PointerDown/Up, no `Button.onClick`, para caminar
  mientras se mantiene presionado). El teclado (WASD/flechas) sigue
  funcionando; ambos inputs se suman en `BirdPlayerController`.
- **Interactuar** y **Picotear** abajo-derecha (`botón.png` + etiqueta).
  Por ahora disparan `Debug.Log` y los `UnityEvent` públicos
  `onInteract`/`onPeck` de `GameHudController` — ahí se engancha la
  lógica de cada room (diálogos, recoger comida, etc.) desde el Inspector.
- **Ruedita** arriba-derecha que abre el mismo panel de Configuración del
  menú (compartido vía `SettingsPanelBuildUtils`) pausando el juego con
  `Time.timeScale = 0`. Escape también lo abre/cierra.
- **Overlay de Brillo** (`BrightnessOverlay`): el slider Brillo ahora
  oscurece la pantalla de verdad, en menú y en juego, en vivo.
- **Etiqueta "Room X" + blink de transición** (`RoomTransitionUI`):
  muestra en qué room estás en todo momento, y hace un blink negro
  rápido en cada cambio de room para que el salto en seco de cámara no
  se sienta como un glitch.

`SettingsManager` ahora es persistente entre escenas (DontDestroyOnLoad +
auto-bootstrap), así que la Configuración funciona igual venga de donde
venga, y expone el evento `Changed` para que futuros consumidores
(AudioManager con los 3 volúmenes, filtro daltónico, vibración móvil) se
actualicen al instante.

Para regenerar todo: `Game > Build All Scenes (Menu + Game)` (o por
separado, `Game > Build Menu Scene` y `Game > Build Game Scene (Cap 1)`).
