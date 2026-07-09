# Sistema de Rooms — Travesía a casa

Sistema de mapa por cuadrículas: cada celda es una pantalla fija (como
las capturas del prototipo de la diseñadora), con transición instantánea
al cruzar un borde. Sin loading screens, con estado persistente de
colectables.

## Instalación

1. Los scripts ya viven en `Assets/Scripts/Rooms/` de este proyecto (si copias esta carpeta a otro proyecto Unity, consérvala en esa misma ruta relativa dentro de `Assets/`).
2. Todo vive en el namespace `TravesiaACasa.Rooms` — no debería chocar con nada existente.

## Setup en el Editor (paso a paso)

### 1. Crear las RoomData

`Assets > Create > Game > RoomData` — una por cada cuadrícula del mapa
(A1, A2, B1... según tu boceto). Asígnale un `roomId` único y su
`gridPosition` (solo referencial).

### 2. Armar cada roomPrefab

Por cada RoomData necesitas un Prefab que contenga:
- El fondo ilustrado (Sprite/Image de esa celda específica).
- Los colliders del escenario (límites caminables).
- Los objetos `CollectibleItem` de esa celda (flor, tapa de botella, caracol, etc.), cada uno con su `itemId` único e `inventoryKey`.
- 4 GameObjects vacíos hijos, nombrados exactamente:
  - `Entry_North`, `Entry_South`, `Entry_East`, `Entry_West`
  
  Solo crea los que correspondan a bordes realmente conectados. Marcan dónde aparece el jugador al entrar por ese lado.
- 4 triggers de borde con `RoomEdgeTrigger` (uno por lado del mapa de esa celda), cada uno con su `Direction` asignada en el Inspector.

Arrastra ese Prefab al campo `roomPrefab` de la RoomData correspondiente.

### 3. Conectar las RoomData entre sí

En cada RoomData, arrastra en los campos `north/south/east/west` la
RoomData vecina según tu boceto del mapa. Si un borde no tiene vecino
(borde del mapa, o zona bloqueada por historia), déjalo vacío — el
`RoomEdgeTrigger` de ese lado simplemente no hará nada.

### 4. Escena principal

En tu escena de gameplay, crea 3 GameObjects vacíos con:
- `RoomManager` → asigna `startingRoom` (la primera RoomData) y `player` (el Transform del Yal austral).
- `CollectibleManager`
- `InventoryManager`

Estos tres son singletons (`Instance`), así que basta con uno de cada uno en la escena.

### 5. Conectar el botón de Interactuar/Picotear

Cuando el jugador está sobre un `CollectibleItem` y presiona el botón
correspondiente (visto en tus capturas), llama a `Collect()` sobre ese
item. La forma más simple: detecta con un trigger/raycast cuál
`CollectibleItem` está más cerca y llama `item.Collect()` desde el
script de tu botón de UI.

### 6. Sistema de Misión (opcional, ya queda la base lista)

Para la pantalla de "Recolecta los materiales..." (madera de maitén,
pegamento de larvas, etc.), suscríbete a
`InventoryManager.Instance.ItemCountChanged` para refrescar el UI de
la misión en tiempo real, y usa `HasAtLeast(itemKey, cantidad)` para
saber si ya se completó cada requisito.

## Notas

- El sistema asume cámara 2D fija por room (como tus capturas), sin scroll continuo. Si más adelante quieren scroll suave dentro de una misma room, se puede agregar Cinemachine sin tocar esta base.
- La persistencia de colectables usa `PlayerPrefs` como placeholder simple. Si necesitan guardado más robusto (múltiples slots, nube, etc.) aíslalo reemplazando solo el interior de `CollectibleManager`.

## Menú y escena jugable con el arte entregado

Con el primer lote de arte de la diseñadora (carpeta
`Assets/assets juego/assets juego aves/`) se generaron dos escenas
nuevas, además del prototipo del grafo (ver `GRAFO_README.md`):

- **`Assets/Scenes/MainMenu.unity`** — título, botón Jugar y botón
  Configuración (con su panel: 3 sliders de sonido + brillo y 2
  interruptores, siguiendo el boceto de `configuración.png`).
  Namespace `TravesiaACasa.Menu`, scripts en `Assets/Scripts/Menu/`.
- **`Assets/Scenes/Game.unity`** — el mismo grafo de 9 rooms del
  prototipo pero con el Yal austral real y fondos ilustrados.

Ambas se generan/regeneran desde el menú del Editor:
`Game > Build All Scenes (Menu + Game)` (o por separado con
`Game > Build Menu Scene` / `Game > Build Game Scene (Cap 1)`). Son
re-ejecutables — sobreescriben la escena entera cada vez, así que no
edites esas escenas a mano si vas a volver a correr el generador
(los ajustes de layout viven en el código de `Assets/Editor/`).

Nota: `activeInputHandler` del proyecto está en modo "Input System
Package" puro (no el Input Manager viejo), por eso `BirdPlayerController`
lee `Keyboard.current` en vez de `Input.GetAxis`. El `CubePlayerController`
del prototipo del grafo todavía usa la API vieja y lanzará un error si
lo pruebas en Play — no se tocó para no romper esa escena, pero
tenlo presente si vuelves a jugar `GraphPrototype.unity`.
