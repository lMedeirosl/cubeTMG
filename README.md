# CubeTMG — Cube World x Realm of the Mad God x Trove

Um jogo 3D para **Unity (C#)** que combina:
- 🧊 **Estilo visual e Câmera Livre 3D de Cube World** (câmera orbital livre com mouse, zoom na roda do scroll, mira em crosshair central, personagens e mundos procedurais voxel).
- ⚔️ **Mecânicas e Combate de Realm of the Mad God (RotMG)** (sistema de 8 atributos matemáticos com teto 8/8, 5 classes clássicas, poções permanentes, balas bullet-hell com padrões em anel/espiral, bolsas de saque com White Bags e ciclo de Nexus recall/morte).
- 💎 **Sistema de Raridades, Estrelas, Pérolas e Biomas de Trove** (escala de raridades de *Common* até *Crystal*, forja com estrelas ⭐, pérolas de sabedoria 🔮, moeda Flux e biomas épicos como Candoria/Candyland, Neon City, Fae Wilds e Permafrost!).

---

## 🎮 Controles & Câmera Estilo Cube World

| Ação | Tecla / Comando | Descrição |
|---|---|---|
| **Câmera Livre 3D** | `Mover o Mouse` | Orbita a câmera livremente em 360° no estilo Cube World. |
| **Zoom da Câmera** | `Scroll do Mouse` | Aproxima em terceira pessoa próxima ou afasta para visão tática ampla. |
| **Movimentação** | `W`, `A`, `S`, `D` | Movimenta o herói na direção em que a câmera está olhando. |
| **Atirar (Autofire)** | `Botão Esquerdo` | Dispara projéteis contínuos exatamente onde o crosshair central da mira estiver apontando. |
| **Habilidade de Classe** | `Espaço` ou `Botão Direito` | Ativa a habilidade da classe consumindo Mana (MP). |
| **Liberar Cursor para UI** | `TAB` ou `ALT Esquerdo` | Libera a seta do mouse para gerenciar o inventário, forjar itens com estrelas e clicar em slots. Clique na tela de jogo para travar a câmera livre novamente! |
| **Pegar Loot do Chão** | `E` ou `Clique na Bolsa` | Ao se aproximar de uma bolsa de loot, ela abre no painel lateral. |
| **Nexus Recall** | `R` | Teleporta instantaneamente para o Nexus seguro (salva sua vida a qualquer momento!). |
| **Poção de Vida** | `F` | Restaura instantaneamente 100 de HP. |
| **Poção de Mana** | `V` | Restaura instantaneamente 100 de MP. |
| **Mochila (1-8)** | `1` a `8` | Equipa ou consome rapidamente itens da mochila. |

---

## 💎 Sistema de Raridades & Forja Estilo Trove

### 1. Escala Completa de Raridades de Trove
Todos os itens que caem de inimigos e chefes contam com a hierarquia e as cores clássicas de **Trove**:
- ⚪ **Common** (Cinza)
- 🟢 **Uncommon** (Verde)
- 🔵 **Rare** (Azul)
- 🟣 **Epic** (Roxo)
- 🟠 **Legendary** (Laranja)
- 🔴 **Relic** (Vermelho)
- 🌈 **Resplendent** (Ciano Radiante)
- 🔮 **Shadow** (Violeta Escuro / Void)
- ☀️ **Radiant** (Branco Solar / Dourado)
- ⭐ **Stellar** (Dourado Cósmico Estelar)
- 💎 **Crystal** (Diamante Iridescente)

### 2. Estrelas de Forja (Stars ⭐⭐⭐⭐⭐) & Pérolas de Sabedoria (🔮)
- **Estrelas (Stars)**: Itens podem ter de **0 a 5 Estrelas (★)**. Cada estrela forjada aumenta todos os atributos do item (dano, vida, defesa, cadência) em **+20%**!
- **Pérolas de Sabedoria (Pearls of Wisdom 🔮)**: Até 5 pérolas por item, conferindo mais **+10% de amplificação** por pérola aplicada!
- **Trove Forge Station no HUD**:
  - Selecione um item na sua mochila no painel lateral.
  - Clique em **`[Forge Star]`** consumindo **Flux ⬡** (a moeda clássica de Trove que cai de monstros e chefes).
  - Clique em **`[Pearl]`** para encantar o item com pérolas!

---

## 🗺️ Biomas de Trove & RotMG (Hub de Portais do Nexus)

No centro do **Nexus**, além da fonte de cura, bonecos de treino e altares de troca de classes, há um majestoso semicírculo de portais interdimensionais:

1. 🌀 **The Realm (RotMG)**:
   - **Lowlands**: Planícies verdes com árvores cúbicas, Goblins e Escorpiões venenosos.
   - **Midlands**: Colinas rochosas, ruínas antigas e Esqueletos Arqueiros.
   - **Godlands**: Terreno vulcânico sombrio de obsidiana com os grandes chefes:
     - **The Cube God**: O lendário chefe cubo translúcido com mini-cubos orbitais, disparando anéis em 360° e rajadas pesadas!
     - **Medusa**: Bombas telegrafadas no chão com dano em área e status *Sick* (bloqueia cura).
     - **Beholder**: Olho flutuante gigante disparando estrelas de 14 projéteis com *Blind* e *Daze*.

2. 🍭 **Candoria (Candyland - Trove)**:
   - Terreno de marshmallow rosa e rios de calda de chocolate.
   - Árvores de pirulito em espiral e pedras de jujuba (*gumdrops*).
   - Inimigos: **Gummy Bears** saltitantes e o chefe **The Cupcake King** (disparando estrelas de açúcar e rajadas de chocolate).

3. ⚡ **Neon City (Trove)**:
   - Grade tecnológica escura com faixas de circuito neon ciano e magenta.
   - Torres de dados cibernéticas e **Neon Cyber Drones** disparando lasers velozes.

4. 🍄 **Fae Wilds (Trove)**:
   - Solo místico de bosque encantado com cogumelos luminescentes gigantes e **Fae Tricksters** alados lançando feitiços que aplicam *Dazed*.

5. ❄️ **Permafrost (Trove)**:
   - Placas de gelo glacial, neve e pináculos congelados com **Frost Golems** disparando estilhaços de gelo que paralisam (*Paralyzed*).

*(Todos os biomas contam com portal de retorno para o Nexus e a tecla `R` sempre o transporta de volta imediatamente!)*

---

## ⚔️ As 5 Classes de RotMG

- **Mago (Wizard)**: Cajado duplo e habilidade **Spellbomb** (16 projéteis arcanos em anel no cursor).
- **Guerreiro (Warrior)**: Espada pesada perfurante e **Elmo** (*Berserk* +50% cadência e *Speedy* +50% velocidade).
- **Clérigo (Priest)**: Varinha sagrada de longo alcance e **Tomo** (cura instantânea de 120 HP e buff *Healing* contínuo).
- **Arqueiro (Archer)**: Arco de 3 flechas em leque e **Aljava** (flecha perfurante com efeito *Paralyzed*).
- **Cavaleiro (Knight)**: Espada e **Escudo** (disparo em cone pesado que aplica *Stunned*, impedindo inimigos de atirar).

---

## 📊 Sistema de Atributos & Fórmulas RotMG (8/8 Maxing)

- **ATT (Ataque)**: $\text{Dano} = \text{Dano Base} \times (0.5 + \frac{\text{ATT}}{50})$
- **DEF (Defesa)**: $\max(\text{Dano} \times 0.15, \text{Dano} - \text{DEF})$
- **SPD (Velocidade)**: $5.0 + (\frac{\text{SPD}}{75}) \times 6.0$
- **DEX (Destreza)**: $\text{Cadência} \times (1.5 + 6.5 \times \frac{\text{DEX}}{75})$
- **VIT (Vitalidade)**: $1.0 + (0.12 \times \text{VIT})$ hp/s
- **WIS (Sabedoria)**: $0.5 + (0.06 \times \text{WIS})$ mp/s
- **Poções de Status Permanentes (Blue Bags)**: Poções de Ataque, Defesa, Velocidade, Destreza, Vitalidade, Sabedoria, Vida e Mana para atingir 8/8!

---

## 🚀 Como Executar na Unity

1. Abra a cena `SampleScene` (em `Assets/Scenes/SampleScene.unity`).
2. Clique no botão **Play** ▶️.
3. O `GameManager` carrega automaticamente tudo na hora!
