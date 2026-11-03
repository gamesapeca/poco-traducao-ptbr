using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace PocoTranslation
{
    [BepInPlugin("com.translation.ptbr.poco", "Poco PT-BR Translation Mod", "1.0.0")]
    public class TranslationPlugin : BaseUnityPlugin
    {
        public static string TranslationDir => Path.Combine(Paths.PluginPath, "PocoTranslation");
        public static string TexturesDir => Path.Combine(TranslationDir, "textures");
        public static string FontsDir => Path.Combine(TranslationDir, "fonts");
        public static TMP_FontAsset CustomFallbackFont { get; private set; }
        public static TranslationPlugin Instance { get; private set; }

        public static readonly Dictionary<string, string> ItemNamesPTBR = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Axe", "Machadinho" },
            { "Old Key", "Chave Antiga" },
            { "Bone Key", "Chave de Osso" },
            { "Golden Key", "Chave Dourada" },
            { "Coin", "Moeda" },
            { "Worm Castings", "Cocô de Minhoca" },
            { "Machete", "Facão" },
            { "Snail Slime", "Gosma de Caracol" },
            { "Magenta Blossom", "Flor Magenta" },
            { "Cyan Berries", "Bagas Ciano" },
            { "Yellow Moss", "Musgo Amarelo" },
            { "Magenta Pigment", "Pigmento Magenta" },
            { "Cyan Pigment", "Pigmento Ciano" },
            { "Yellow Pigment", "Pigmento Amarelo" },
            { "Red Paint", "Tinta Vermelha" },
            { "Green Paint", "Tinta Verde" },
            { "Blue Paint", "Tinta Azul" },
            { "Potato Battery", "Bateria de Batata" },
            { "Maggot", "Larva" },
            { "Fish", "Peixe" },
            { "Carrot", "Cenoura" },
            { "Bucket", "Balde" },
            { "Beetle Milk", "Leite de Besouro" },
            { "Frying Pan", "Frigideira" },
            { "Match", "Fósforo" },
            { "Coal", "Carvão" },
            { "Cooked Fish Dinner", "Banquete de Peixe Cozido" },
            { "Spoon", "Colher" },
            { "Rope", "Corda" },
            { "Feather", "Pena" },
            { "Basic Button", "Botão Comum" },
            { "Cute Button", "Botão Fofo" },
            { "Fancy Button", "Botão Chique" },
            { "Cymbal Button", "Botão de Prato" },
            { "Cog", "Engrenagem" },
            { "Valve Handle", "Válvula" },
            { "Pacifier", "Chupeta" },
            { "Bug Steak", "Bife de Inseto" },
            { "Dog Treat", "Petisco de Cachorro" },
            { "Rocket Thruster", "Propulsor do Foguete" },
            { "Rocket Hull", "Fuselagem do Foguete" },
            { "Rocket Tip", "Bico do Foguete" }
        };

        public static readonly Dictionary<string, string> NPCNamesPTBR = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Fisherman", "Pescador" },
            { "Drummer", "Baterista" },
            { "Addict", "Viciado" },
            { "Caged", "Enjaulado" },
            { "Caged Beetle", "Besouro Enjaulado" },
            { "CagedBeetle", "Besouro Enjaulado" },
            { "Worm", "Minhoca" },
            { "Worm Cop", "Minhoca Policial" },
            { "Cop Worm", "Minhoca Policial" },
            { "Cop", "Policial" },
            { "Worm Widow", "Viúva Minhoca" },
            { "Wife Worm", "Esposa Minhoca" },
            { "Wife", "Esposa" },
            { "Widow", "Viúva" },
            { "Bird", "Pássaro" },
            { "Clown", "Palhaço" },
            { "Cerberus", "Cérbero" },
            { "NPC", "Habitante" },
            { "Poco", "Poco" },
            { "Gultch", "Gultch" },
            { "Gripp", "Gripp" },
            { "Nari", "Nari" },
            { "Jaz", "Jaz" },
            { "Ojet", "Ojet" },
            { "John", "John" },
            { "Dungsworth", "Dungsworth" },
            { "Scuttlesby", "Scuttlesby" },
            { "Weevilton", "Weevilton" }
        };

        public static readonly Dictionary<string, string> GlobalUIMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Settings and options UI
            { "Settings", "Opções" },
            { "Options", "Opções" },
            { "Resolution", "Resolução" },
            { "Fullscreen", "Tela Cheia" },
            { "Volume", "Volume" },
            { "Music", "Música" },
            { "Sound", "Efeitos Sonoros" },
            { "Voices", "Vozes" },
            { "Music Volume", "Volume da Música" },
            { "Sound Volume", "Volume dos Efeitos" },
            { "Audio Adjustments", "Ajustes de Áudio" },
            { "Text Crawl Enabled", "Texto Gradual" },
            { "Always Run", "Sempre Correr" },
            { "Done", "Pronto" },
            { "Cursor", "Cursor" },
            { "Lefty", "Canhoto" },
            { "Drag Amount", "Sensibilidade" },
            { "Cursor Help", "Ajuda do Cursor" },
            { "Quit to Menu", "Menu Principal" },
            { "Quit to Desktop", "Sair do Jogo" },

            // Hints, map and exploration
            { "Hint", "Dica" },
            { "Hint?", "Dica?" },
            { "Hints", "Dicas" },
            { "Hint Button", "Botão de Dica" },
            { "Hint Circle", "Círculo de Dica" },
            { "Map", "Mapa" },
            { "Compass", "Bússola" },
            { "Photo", "Foto" },
            { "Photos", "Fotos" },
            { "Piece", "Pedaço" },
            { "Pieces", "Pedaços" },
            { "Photo Pieces", "Pedaços de Foto" },
            { "Clear Map", "Limpar Mapa" },

            // Main and pause menus
            { "Start", "Iniciar" },
            { "Start New Game", "Novo Jogo" },
            { "Continue", "Continuar" },
            { "Continue Game", "Continuar Jogo" },
            { "Quit", "Sair" },
            { "Restart Demo", "Reiniciar Demonstração" },
            { "Back", "Voltar" },
            { "Reset", "Redefinir" },
            { "How to Play:", "Como Jogar:" },
            { "Inventory", "Inventário" },
            { "Inventory Empty", "Inventário Vazio" },
            { "Item Name - Item Description", "Nome do Item - Descrição do Item" },
            { "Item Name", "Nome do Item" },
            { "Item Description", "Descrição do Item" },
            { "Description", "Descrição" },
            { "Use", "Usar" },
            { "Combine", "Combinar" },
            { "Insert", "Inserir" },
            { "Clear", "Limpar" },
            { "Close", "Fechar" },
            { "Open", "Abrir" },

            // Interaction prompts (ItemPromptPoints)
            { "Get the foot to move...", "Faça o pé se mover..." },
            { "Get the foot to move", "Faça o pé se mover" },
            { "The door needs a key...", "A porta precisa de uma chave..." },
            { "The door needs a key", "A porta precisa de uma chave" },
            { "Add a pigment...", "Adicione um pigmento..." },
            { "Add a pigment", "Adicione um pigmento" },
            { "Add some fuel...", "Adicione combustível..." },
            { "Add some fuel", "Adicione combustível" },
            { "Attach an item...", "Prenda um item..." },
            { "Attach an item", "Prenda um item" },
            { "Bridge the gap...", "Cruze o abismo..." },
            { "Bridge the gap", "Cruze o abismo" },
            { "Choose a binder...", "Escolha uma base..." },
            { "Choose a binder", "Escolha uma base" },
            { "Choose an item to mash...", "Escolha um item para esmagar..." },
            { "Choose an item to mash", "Escolha um item para esmagar" },
            { "Clear the path...", "Desobstrua o caminho..." },
            { "Clear the path", "Desobstrua o caminho" },
            { "Light the fire...", "Acenda o fogo..." },
            { "Light the fire", "Acenda o fogo" },
            { "Needs fertilizer...", "Precisa de adubo..." },
            { "Needs fertilizer", "Precisa de adubo" },
            { "Needs something to row with...", "Precisa de algo para remar..." },
            { "Needs something to row with", "Precisa de algo para remar" },
            { "Offer an item...", "Ofereça um item..." },
            { "Offer an item", "Ofereça um item" },
            { "Place a button...", "Coloque um botão..." },
            { "Place a button", "Coloque um botão" },
            { "Place an item atop the grill...", "Coloque um item na churrasqueira..." },
            { "Place an item atop the grill", "Coloque um item na churrasqueira" },
            { "Plug something in...", "Conecte algo aqui..." },
            { "Plug something in", "Conecte algo aqui" },
            { "Put something in the slot...", "Coloque algo na abertura..." },
            { "Put something in the slot", "Coloque algo na abertura" },
            { "Use an item on the beetle...", "Use um item no besouro..." },
            { "Use an item on the beetle", "Use um item no besouro" },

            // Confirmation and support screens
            { "Are you sure you want to start a new game?\nYou will lose all your current progress.", "Tem certeza de que deseja iniciar um novo jogo?\nTodo o seu progresso atual será perdido." },
            { "Yes, Start", "Sim, Iniciar" },
            { "No, Cancel", "Não, Cancelar" },
            { "Yes", "Sim" },
            { "No", "Não" },
            { "Cancel", "Cancelar" },
            { "Donate", "Apoiar" },
            { "You can donate to us on Ko-fi at:\nko-fi.com/whalefallgames", "Você pode nos apoiar no Ko-fi em:\nko-fi.com/whalefallgames" },
            { "Open In Browser", "Abrir no Navegador" },

            // World actions and prompts
            { "Choose an Item to Use...", "Escolha um Item para Usar..." },
            { "Choose an Item to Insert...", "Escolha um Item para Inserir..." },
            { "Insert Item", "Inserir Item" },
            { "Click on Poco, Right Click, or press Esc to view Inventory/Map/Settings.", "Clique no Poco, clique com o Botão Direito ou pressione Esc para abrir Inventário/Mapa/Opções." },
            { "<b>Click</b> on Poco, Press <b>Esc</b>, or <b>Right Click</b> to Open Menu", "<b>Clique</b> no Poco, pressione <b>Esc</b> ou use o <b>Botão Direito</b> para abrir o Menu" },
            { "Point and click with the mouse to walk around.", "Aponte e clique com o mouse para andar pelo cenário." },
            { "When the glove cursor changes, you can click to interact.", "Quando o cursor de luva mudar de forma, clique para interagir." },
            { "Travel", "Viajar" },
            { "Talk", "Conversar" },
            { "Interact", "Interagir" },
            { "Enter screen name...", "Digite o nome da tela..." },
            { "Enter item name...", "Digite o nome do item..." },
            { "Enter text...", "Digite o texto..." },

            // Developer options
            { "Debug Mode", "Modo de Depuração" },
            { "Get Item", "Obter Item" },
            { "Get All Rats", "Obter Todos os Ratos" },
            { "CLEAR SAVES", "APAGAR DADOS SALVOS" },
            { "ClearSaves", "Apagar Dados Salvos" },
            { "Poco Response Enabled", "Respostas do Poco Ativadas" },
            { "Finish Game", "Concluir Jogo" },
            { "Okay", "OK" },
            { "Option 1", "Opção 1" },
            { "Option 2", "Opção 2" },
            { "Option 3", "Opção 3" },
            { "Option 4", "Opção 4" },
            { "Option A", "Opção A" },
            { "Option B", "Opção B" },
            { "Option C", "Opção C" }
        };

        public static readonly Dictionary<string, string> CreditsHeaderMap = new Dictionary<string, string>
        {
            { "DIRECTOR", "DIREÇÃO" },
            { "PRODUCER", "PRODUÇÃO" },
            { "PRODUCTION ASSISTANT", "ASSISTÊNCIA DE PRODUÇÃO" },
            { "CORE TEAM", "EQUIPE PRINCIPAL" },
            { "VISUAL DEVELOPMENT LEAD", "LIDERANÇA DE DESENVOLVIMENTO VISUAL" },
            { "VISUAL DEVELOPMENT ARTISTS", "ARTISTAS DE DESENVOLVIMENTO VISUAL" },
            { "PAINTERS", "PINTORES" },
            { "STORY LEAD", "LIDERANÇA DE ROTEIRO" },
            { "ANIMATION LEAD", "LIDERANÇA DE ANIMAÇÃO" },
            { "ANIMATORS", "ANIMADORES" },
            { "MODELING LEAD", "LIDERANÇA DE MODELAGEM" },
            { "MODELER", "MODELAGEM" },
            { "RIGGING LEAD", "LIDERANÇA DE RIGGING" },
            { "RIGGING ARTIST", "ARTISTA DE RIGGING" },
            { "PROGRAMMER", "PROGRAMAÇÃO" },
            { "SOUND DESIGN", "DESIGN DE SOM" },
            { "MUSIC", "TRILHA SONORA" },
            { "VOICE ACTORS", "DUBLADORES" },
            { "SOCIAL MEDIA MANAGER", "GERÊNCIA DE MÍDIAS SOCIAIS" },
            { "VOLUNTEERS", "VOLUNTÁRIOS" },
            { "SOCIAL MEDIA COORDINATOR", "COORDENAÇÃO DE MÍDIAS SOCIAIS" },
            { "BACKGROUND PAINTERS", "PINTORES DE CENÁRIO" },
            { "KICKSTARTER BACKERS", "APOIADORES DO KICKSTARTER" },
            { "POCOSONA BACKERS", "APOIADORES POCOSONA" },
            { "FACULTY ADVISOR", "ORIENTAÇÃO ACADÊMICA" },
            { "SPECIAL THANKS", "AGRADECIMENTOS ESPECIAIS" },
            { "CLOWN CONSULTANT", "CONSULTORIA DE PALHAÇO" },
            { "Poco - Bike Horn", "Poco - Buzina de Bicicleta" }
        };

        private void Awake()
        {
            Instance = this;
            EnsureCustomFontFallback();
            var harmony = new Harmony("com.translation.ptbr.poco");
            harmony.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
            gameObject.AddComponent<ArtTooltipController>();
            Logger.LogInfo("Poco PT-BR Translation Mod loaded successfully.");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureCustomFontFallback();
            if (CustomFallbackFont != null)
            {
                InjectFallbackFont(CustomFallbackFont);
            }
            TranslateAllSceneTexts();
            ApplyCustomTextures();
        }

        public void EnsureCustomFontFallback()
        {
            if (CustomFallbackFont != null) return;

            try
            {
                string fontPath = Path.Combine(FontsDir, "Poco-PTBR.ttf");
                if (!File.Exists(fontPath))
                {
                    fontPath = Path.Combine(FontsDir, "Sniglet-Regular.ttf");
                }
                if (!File.Exists(fontPath))
                {
                    Logger.LogWarning("[PocoTranslation] Font not found at: " + fontPath);
                    return;
                }

                TMP_FontAsset asset = null;

                try
                {
                    Font font = new Font();
                    var method = typeof(Font).GetMethod("Internal_CreateFontFromPath", BindingFlags.NonPublic | BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { font, fontPath });
                    }
                    asset = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("[PocoTranslation] Font asset creation via Font instance failed: " + ex.Message);
                }

                if (asset != null)
                {
                    asset.name = "Poco-PTBR SDF";
                    DontDestroyOnLoad(asset);
                    if (asset.material != null) DontDestroyOnLoad(asset.material);

                    string charsToPreload = "áéíóúâêôãõçàèìòùäëïöüñÁÉÍÓÚÂÊÔÃÕÇÀÈÌÒÙÄËÏÖÜÑ" +
                                           "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                           "0123456789.,:;!?\"'()[]{}-+*/=_@#$%^&~`|\\<>«»—–…ºª";
                    asset.TryAddCharacters(charsToPreload);

                    SynchronizeFaceInfo(asset);

                    CustomFallbackFont = asset;
                    Logger.LogInfo("[PocoTranslation] Custom fallback font Poco-PTBR SDF initialized with scale " + asset.faceInfo.scale);

                    InjectFallbackFont(CustomFallbackFont);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[PocoTranslation] Failed to initialize custom font fallback: " + ex);
            }
        }

        public static void SynchronizeFaceInfo(TMP_FontAsset target)
        {
            if (target == null) return;

            try
            {
                TMP_FontAsset pocoOrig = null;
                var allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                foreach (var f in allFonts)
                {
                    if (f != null && f != target && !string.IsNullOrEmpty(f.name) && f.name.IndexOf("Poco-Regular", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        pocoOrig = f;
                        break;
                    }
                }

                var fi = target.faceInfo;
                if (pocoOrig != null)
                {
                    fi.pointSize = pocoOrig.faceInfo.pointSize;
                    fi.scale = pocoOrig.faceInfo.scale;
                    fi.lineHeight = pocoOrig.faceInfo.lineHeight;
                    fi.ascentLine = pocoOrig.faceInfo.ascentLine;
                    fi.capLine = pocoOrig.faceInfo.capLine;
                    fi.meanLine = pocoOrig.faceInfo.meanLine;
                    fi.baseline = pocoOrig.faceInfo.baseline;
                    fi.descentLine = pocoOrig.faceInfo.descentLine;
                    fi.underlineOffset = pocoOrig.faceInfo.underlineOffset;
                    fi.underlineThickness = pocoOrig.faceInfo.underlineThickness;
                    fi.strikethroughOffset = pocoOrig.faceInfo.strikethroughOffset;
                    fi.strikethroughThickness = pocoOrig.faceInfo.strikethroughThickness;
                    fi.superscriptOffset = pocoOrig.faceInfo.superscriptOffset;
                    fi.superscriptSize = pocoOrig.faceInfo.superscriptSize;
                    fi.subscriptOffset = pocoOrig.faceInfo.subscriptOffset;
                    fi.subscriptSize = pocoOrig.faceInfo.subscriptSize;
                    fi.tabWidth = pocoOrig.faceInfo.tabWidth;
                }
                else
                {
                    fi.pointSize = 90;
                    fi.scale = 1.5f;
                    fi.lineHeight = 83.88f;
                    fi.ascentLine = 68.22f;
                    fi.capLine = 57f;
                    fi.meanLine = 46f;
                    fi.baseline = 0f;
                    fi.descentLine = -7.56f;
                }
                target.faceInfo = fi;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[PocoTranslation] Error synchronizing face info: " + ex.Message);
            }
        }

        public static void InjectFallbackFont(TMP_FontAsset fallbackAsset)
        {
            if (fallbackAsset == null) return;

            SynchronizeFaceInfo(fallbackAsset);

            if (TMP_Settings.fallbackFontAssets != null)
            {
                if (!TMP_Settings.fallbackFontAssets.Contains(fallbackAsset))
                {
                    TMP_Settings.fallbackFontAssets.Insert(0, fallbackAsset);
                }
            }

            var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            foreach (var font in fonts)
            {
                if (font == null || font == fallbackAsset) continue;
                if (font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) >= 0) continue;

                if (font.fallbackFontAssetTable == null)
                {
                    font.fallbackFontAssetTable = new List<TMP_FontAsset>();
                }
                if (!font.fallbackFontAssetTable.Contains(fallbackAsset))
                {
                    font.fallbackFontAssetTable.Insert(0, fallbackAsset);
                }
            }
        }

        private static readonly Dictionary<string, Sprite> CustomSpriteCache = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

        public void ApplyCustomTextures()
        {
            try
            {
                if (!Directory.Exists(TexturesDir)) return;

                // Handle 2D SpriteRenderers
                foreach (var sr in Resources.FindObjectsOfTypeAll<SpriteRenderer>())
                {
                    if (sr == null || sr.sprite == null || sr.sprite.texture == null) continue;

                    string texName = sr.sprite.texture.name;
                    string pngPath = Path.Combine(TexturesDir, texName + ".png");
                    if (!File.Exists(pngPath))
                    {
                        string spriteName = sr.sprite.name;
                        pngPath = Path.Combine(TexturesDir, spriteName + ".png");
                        if (!File.Exists(pngPath)) continue;
                        texName = spriteName;
                    }

                    if (!CustomSpriteCache.TryGetValue(texName, out Sprite customSprite))
                    {
                        byte[] imgData = File.ReadAllBytes(pngPath);
                        Texture2D newTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (ImageConversion.LoadImage(newTex, imgData))
                        {
                            newTex.name = texName + "_PTBR";
                            Sprite orig = sr.sprite;
                            customSprite = Sprite.Create(
                                newTex,
                                new Rect(0f, 0f, newTex.width, newTex.height),
                                new Vector2(orig.pivot.x / orig.rect.width, orig.pivot.y / orig.rect.height),
                                orig.pixelsPerUnit,
                                0,
                                SpriteMeshType.FullRect,
                                orig.border
                            );
                            CustomSpriteCache[texName] = customSprite;
                        }
                    }

                    if (customSprite != null && sr.sprite != customSprite)
                    {
                        sr.sprite = customSprite;
                    }
                }

                // Handle UI Canvas Images (e.g. HintButton)
                foreach (var img in Resources.FindObjectsOfTypeAll<Image>())
                {
                    if (img == null || img.sprite == null || img.sprite.texture == null) continue;

                    string texName = img.sprite.texture.name;
                    string pngPath = Path.Combine(TexturesDir, texName + ".png");
                    if (!File.Exists(pngPath))
                    {
                        string spriteName = img.sprite.name;
                        pngPath = Path.Combine(TexturesDir, spriteName + ".png");
                        if (!File.Exists(pngPath)) continue;
                        texName = spriteName;
                    }

                    if (!CustomSpriteCache.TryGetValue(texName, out Sprite customSprite))
                    {
                        byte[] imgData = File.ReadAllBytes(pngPath);
                        Texture2D newTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (ImageConversion.LoadImage(newTex, imgData))
                        {
                            newTex.name = texName + "_PTBR";
                            Sprite orig = img.sprite;
                            customSprite = Sprite.Create(
                                newTex,
                                new Rect(0f, 0f, newTex.width, newTex.height),
                                new Vector2(orig.pivot.x / orig.rect.width, orig.pivot.y / orig.rect.height),
                                orig.pixelsPerUnit,
                                0,
                                SpriteMeshType.FullRect,
                                orig.border
                            );
                            CustomSpriteCache[texName] = customSprite;
                        }
                    }

                    if (customSprite != null && img.sprite != customSprite)
                    {
                        img.sprite = customSprite;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[PocoTranslation] Error applying custom textures: " + ex.Message);
            }
        }

        public void TranslateAllSceneTexts()
        {
            try
            {
                if (CustomFallbackFont != null)
                {
                    SynchronizeFaceInfo(CustomFallbackFont);
                }

                foreach (var tmp in Resources.FindObjectsOfTypeAll<TMP_Text>())
                {
                    if (tmp != null)
                    {
                        if (CustomFallbackFont != null && tmp.font != null && tmp.font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            var table = tmp.font.fallbackFontAssetTable;
                            if (table == null)
                            {
                                table = new List<TMP_FontAsset>();
                                tmp.font.fallbackFontAssetTable = table;
                            }
                            if (!table.Contains(CustomFallbackFont))
                            {
                                table.Insert(0, CustomFallbackFont);
                            }
                        }

                        if (!string.IsNullOrEmpty(tmp.text))
                        {
                            string loc = LocalizeText(tmp.text);
                            if (loc != tmp.text)
                            {
                                tmp.text = loc;
                                tmp.SetVerticesDirty();
                                tmp.SetLayoutDirty();
                                tmp.ForceMeshUpdate(true);
                            }
                        }
                    }
                }

                foreach (var txt in Resources.FindObjectsOfTypeAll<Text>())
                {
                    if (txt != null && !string.IsNullOrEmpty(txt.text))
                    {
                        string loc = LocalizeText(txt.text);
                        if (loc != txt.text)
                        {
                            txt.text = loc;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[PocoTranslation] Warning during scene text scan: " + ex.Message);
            }
        }

        public static string StripAccents(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public static string LocalizeText(string original)
        {
            if (string.IsNullOrEmpty(original)) return original;

            string trimmed = original.Trim();

            // End credits formatting
            if (original.Contains("DIRECTOR") && original.Contains("Micah Boursier"))
            {
                return LocalizeCredits(original);
            }

            // Direct mapping
            if (GlobalUIMap.TryGetValue(trimmed, out string localized))
            {
                return localized;
            }

            // Line break normalization
            string normalized = trimmed.Replace("\r\n", "\n");
            if (GlobalUIMap.TryGetValue(normalized, out string locNorm))
            {
                return locNorm;
            }

            // Punctuation suffix handling
            if (trimmed.EndsWith("?") && trimmed.Length > 1)
            {
                string without = trimmed.Substring(0, trimmed.Length - 1).Trim();
                if (GlobalUIMap.TryGetValue(without, out string locWithout))
                {
                    return locWithout + "?";
                }
            }

            if (trimmed.EndsWith(":") && trimmed.Length > 1)
            {
                string without = trimmed.Substring(0, trimmed.Length - 1).Trim();
                if (GlobalUIMap.TryGetValue(without, out string locWithout))
                {
                    return locWithout + ":";
                }
            }

            if (trimmed.EndsWith("...") && trimmed.Length > 3)
            {
                string without = trimmed.Substring(0, trimmed.Length - 3).Trim();
                if (GlobalUIMap.TryGetValue(without, out string locWithout))
                {
                    return locWithout + "...";
                }
            }

            // Item name mapping
            if (ItemNamesPTBR.TryGetValue(trimmed, out string localizedItem))
            {
                return localizedItem;
            }

            // NPC name mapping
            if (NPCNamesPTBR.TryGetValue(trimmed, out string localizedNpc))
            {
                return localizedNpc;
            }

            return original;
        }

        public static string LocalizeCredits(string original)
        {
            string result = original;
            foreach (var kvp in CreditsHeaderMap)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }

            if (!result.Contains("gamesapeca"))
            {
                result += "\n\n\nLOCALIZAÇÃO PARA PORTUGUÊS-BR\ngamesapeca\n";
            }

            return result;
        }

        public static List<Dictionary<string, object>> ParseCsvText(string csvText)
        {
            var list = new List<Dictionary<string, object>>();
            string SPLIT_RE = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            string LINE_SPLIT_RE = "\r\n|\n\r|\n|\r";
            char[] TRIM_CHARS = new char[] { '"' };

            string[] lines = Regex.Split(csvText, LINE_SPLIT_RE);
            if (lines.Length <= 1) return list;

            string[] headers = Regex.Split(lines[0], SPLIT_RE);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                string[] cols = Regex.Split(lines[i], SPLIT_RE);
                var dict = new Dictionary<string, object>();
                for (int j = 0; j < headers.Length && j < cols.Length; j++)
                {
                    string text = cols[j].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\"\"", "\"").Replace("\\", "");
                    object value = text;
                    if (int.TryParse(text, out int intVal)) value = intVal;
                    else if (float.TryParse(text, out float floatVal)) value = floatVal;
                    dict[headers[j]] = value;
                }
                list.Add(dict);
            }
            return list;
        }
    }

    [HarmonyPatch(typeof(CSVReader), "Read")]
    public static class Patch_CSVReader_Read
    {
        static bool Prefix(string file, ref List<Dictionary<string, object>> __result)
        {
            try
            {
                string csvFile = Path.Combine(TranslationPlugin.TranslationDir, file + ".csv");
                if (File.Exists(csvFile))
                {
                    string content = File.ReadAllText(csvFile, System.Text.Encoding.UTF8);
                    __result = TranslationPlugin.ParseCsvText(content);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[PocoTranslation] Error loading custom CSV " + file + ": " + ex.Message);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "SetHighlightedItem")]
    public static class Patch_PlayerController_SetHighlightedItem
    {
        static void Prefix(ref string name, ref string desc)
        {
            if (!string.IsNullOrEmpty(name) && TranslationPlugin.ItemNamesPTBR.TryGetValue(name, out string localizedName))
            {
                name = localizedName;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerController), "SetTutorial")]
    public static class Patch_PlayerController_SetTutorial
    {
        static void Prefix(ref string tut)
        {
            if (!string.IsNullOrEmpty(tut) && TranslationPlugin.GlobalUIMap.TryGetValue(tut, out string localizedTut))
            {
                tut = localizedTut;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerController), "MainMenuOpenSettings")]
    public static class Patch_PlayerController_MainMenuOpenSettings
    {
        static void Postfix()
        {
            TranslationPlugin.Instance?.TranslateAllSceneTexts();
        }
    }

    [HarmonyPatch(typeof(PlayerController), "OpenMenu")]
    public static class Patch_PlayerController_OpenMenu
    {
        static void Postfix()
        {
            TranslationPlugin.Instance?.TranslateAllSceneTexts();
        }
    }

    [HarmonyPatch(typeof(PlayerController), "SwitchMenuTab")]
    public static class Patch_PlayerController_SwitchMenuTab
    {
        static void Postfix()
        {
            TranslationPlugin.Instance?.TranslateAllSceneTexts();
        }
    }

    [HarmonyPatch(typeof(WorldController), "SetMainMenuOpen")]
    public static class Patch_WorldController_SetMainMenuOpen
    {
        static void Postfix()
        {
            TranslationPlugin.Instance?.TranslateAllSceneTexts();
        }
    }

    [HarmonyPatch(typeof(WorldController), "Start")]
    public static class Patch_WorldController_Start
    {
        static void Postfix(WorldController __instance)
        {
            try
            {
                if (TranslationPlugin.CustomFallbackFont != null)
                {
                    TranslationPlugin.InjectFallbackFont(TranslationPlugin.CustomFallbackFont);
                }
                ApplyDialogueLayoutFixes(__instance);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[PocoTranslation] Error in WorldController.Start patch: " + ex.Message);
            }
        }

        public static void ApplyDialogueLayoutFixes(WorldController wc)
        {
            if (wc == null) return;

            var dText = Traverse.Create(wc).Field<TMP_Text>("dialogueText")?.Value;
            if (dText != null)
            {
                dText.enableAutoSizing = true;
                dText.fontSizeMin = 20f;
                dText.fontSizeMax = 32f;
                dText.margin = new Vector4(20f, 15f, 65f, 20f);
            }

            var dOptions = Traverse.Create(wc).Field<Transform>("dialogueOptionContent")?.Value;
            if (dOptions != null)
            {
                for (int i = 1; i <= 4; i++)
                {
                    var opt = dOptions.Find("Option" + i);
                    if (opt != null)
                    {
                        var textTrans = opt.Find("Text");
                        if (textTrans != null)
                        {
                            var tmp = textTrans.GetComponent<TMP_Text>();
                            if (tmp != null)
                            {
                                tmp.enableWordWrapping = false;
                                tmp.enableAutoSizing = true;
                                tmp.fontSizeMin = 16f;
                                tmp.fontSizeMax = 28f;
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorldController), "PopulateDialogueBox")]
    public static class Patch_WorldController_PopulateDialogueBox
    {
        static void Postfix(WorldController __instance)
        {
            try
            {
                if (__instance != null)
                {
                    var dText = Traverse.Create(__instance).Field<TMP_Text>("dialogueText")?.Value;
                    if (dText != null)
                    {
                        var nameBox = dText.transform.parent?.Find("NameTag")?.Find("NameBox")?.GetComponent<TMP_Text>();
                        if (nameBox != null)
                        {
                            if (TranslationPlugin.CustomFallbackFont != null && nameBox.font != null)
                            {
                                var nTable = nameBox.font.fallbackFontAssetTable;
                                if (nTable == null)
                                {
                                    nTable = new List<TMP_FontAsset>();
                                    nameBox.font.fallbackFontAssetTable = nTable;
                                }
                                if (!nTable.Contains(TranslationPlugin.CustomFallbackFont))
                                {
                                    nTable.Insert(0, TranslationPlugin.CustomFallbackFont);
                                }
                            }

                            if (!string.IsNullOrEmpty(nameBox.text))
                            {
                                string curName = nameBox.text.Trim();
                                if (TranslationPlugin.NPCNamesPTBR.TryGetValue(curName, out string transName))
                                {
                                    nameBox.text = transName;
                                    nameBox.SetVerticesDirty();
                                    nameBox.SetLayoutDirty();
                                }
                            }
                        }

                        if (TranslationPlugin.CustomFallbackFont != null && dText.font != null)
                        {
                            var table = dText.font.fallbackFontAssetTable;
                            if (table == null)
                            {
                                table = new List<TMP_FontAsset>();
                                dText.font.fallbackFontAssetTable = table;
                            }
                            if (!table.Contains(TranslationPlugin.CustomFallbackFont))
                            {
                                table.Insert(0, TranslationPlugin.CustomFallbackFont);
                            }
                        }

                        Patch_WorldController_Start.ApplyDialogueLayoutFixes(__instance);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[PocoTranslation] Error in WorldController.PopulateDialogueBox patch: " + ex.Message);
            }
        }
    }

    [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
    public static class Patch_TMPText_text
    {
        static void Prefix(TMP_Text __instance, ref string value)
        {
            if (value != null)
            {
                value = TranslationPlugin.LocalizeText(value);

                if (__instance != null && __instance.font != null && !string.IsNullOrEmpty(__instance.font.name))
                {
                    if (__instance.font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        value = TranslationPlugin.StripAccents(value);
                    }
                    else if (TranslationPlugin.CustomFallbackFont != null)
                    {
                        var table = __instance.font.fallbackFontAssetTable;
                        if (table == null)
                        {
                            table = new List<TMP_FontAsset>();
                            __instance.font.fallbackFontAssetTable = table;
                        }
                        if (!table.Contains(TranslationPlugin.CustomFallbackFont))
                        {
                            table.Insert(0, TranslationPlugin.CustomFallbackFont);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Text), "text", MethodType.Setter)]
    public static class Patch_LegacyText_text
    {
        static void Prefix(ref string value)
        {
            if (value != null)
            {
                value = TranslationPlugin.LocalizeText(value);
            }
        }
    }

    [HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
    public static class Patch_TextMeshProUGUI_OnEnable
    {
        static void Postfix(TextMeshProUGUI __instance)
        {
            if (__instance != null)
            {
                if (TranslationPlugin.CustomFallbackFont != null && __instance.font != null &&
                    __instance.font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    var table = __instance.font.fallbackFontAssetTable;
                    if (table == null)
                    {
                        table = new List<TMP_FontAsset>();
                        __instance.font.fallbackFontAssetTable = table;
                    }
                    if (!table.Contains(TranslationPlugin.CustomFallbackFont))
                    {
                        table.Insert(0, TranslationPlugin.CustomFallbackFont);
                    }
                }

                if (!string.IsNullOrEmpty(__instance.text))
                {
                    string loc = TranslationPlugin.LocalizeText(__instance.text);
                    if (__instance.font != null && !string.IsNullOrEmpty(__instance.font.name) &&
                        __instance.font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        loc = TranslationPlugin.StripAccents(loc);
                    }
                    if (loc != __instance.text)
                    {
                        __instance.text = loc;
                        __instance.SetVerticesDirty();
                        __instance.SetLayoutDirty();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TextMeshPro), "OnEnable")]
    public static class Patch_TextMeshPro_OnEnable
    {
        static void Postfix(TextMeshPro __instance)
        {
            if (__instance != null)
            {
                if (TranslationPlugin.CustomFallbackFont != null && __instance.font != null &&
                    __instance.font.name.IndexOf("jinglejangle", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    var table = __instance.font.fallbackFontAssetTable;
                    if (table == null)
                    {
                        table = new List<TMP_FontAsset>();
                        __instance.font.fallbackFontAssetTable = table;
                    }
                    if (!table.Contains(TranslationPlugin.CustomFallbackFont))
                    {
                        table.Insert(0, TranslationPlugin.CustomFallbackFont);
                    }
                }

                if (!string.IsNullOrEmpty(__instance.text))
                {
                    string loc = TranslationPlugin.LocalizeText(__instance.text);
                    if (loc != __instance.text)
                    {
                        __instance.text = loc;
                        __instance.SetVerticesDirty();
                        __instance.SetLayoutDirty();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Text), "OnEnable")]
    public static class Patch_LegacyText_OnEnable
    {
        static void Postfix(Text __instance)
        {
            if (__instance != null && !string.IsNullOrEmpty(__instance.text))
            {
                string loc = TranslationPlugin.LocalizeText(__instance.text);
                if (loc != __instance.text)
                {
                    __instance.text = loc;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ItemPromptPoint), "GetPromptHint")]
    public static class Patch_ItemPromptPoint_GetPromptHint
    {
        static void Postfix(ref string __result)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                __result = TranslationPlugin.LocalizeText(__result);
            }
        }
    }

    [HarmonyPatch(typeof(MapController), "MapToggle")]
    public static class Patch_MapController_MapToggle
    {
        static void Postfix(bool b)
        {
            if (b && TranslationPlugin.Instance != null)
            {
                TranslationPlugin.Instance.ApplyCustomTextures();
            }
        }
    }

    [HarmonyPatch(typeof(MapController), "Start")]
    public static class Patch_MapController_Start
    {
        static void Postfix()
        {
            TranslationPlugin.Instance?.ApplyCustomTextures();
        }
    }

    [HarmonyPatch(typeof(Image), "OnEnable")]
    public static class Patch_Image_OnEnable
    {
        static void Postfix(Image __instance)
        {
            if (__instance != null && __instance.sprite != null && __instance.sprite.texture != null)
            {
                string name = __instance.sprite.texture.name;
                if (string.Equals(name, "HintButton", StringComparison.OrdinalIgnoreCase))
                {
                    TranslationPlugin.Instance?.ApplyCustomTextures();
                }
            }
        }
    }

    public class ArtTooltipController : MonoBehaviour
    {
        private GUIStyle panelStyle;
        private GUIStyle bodyStyle;
        private Texture2D panelBgTexture;
        private Texture2D dividerTexture;

        private string readingPanelContent = "";

        private void Awake()
        {
            panelBgTexture = new Texture2D(1, 1);
            panelBgTexture.SetPixel(0, 0, new Color(0.07f, 0.06f, 0.05f, 0.94f));
            panelBgTexture.Apply();

            dividerTexture = new Texture2D(1, 1);
            dividerTexture.SetPixel(0, 0, new Color(0.72f, 0.60f, 0.38f, 0.50f));
            dividerTexture.Apply();
        }

        private void Update()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            // Dead Beetle scene document
            if (sceneName.Contains("DeadBeetle"))
            {
                GameObject noteObj = GameObject.Find("scn_DeadBeetle_03");
                if (noteObj != null && noteObj.activeInHierarchy)
                {
                    readingPanelContent = "<i>Aos meus companheiros de banda,</i>\n\n" +
                        "Sinto muita saudade de todos vocês.\n" +
                        "Não sei para onde foram, mas deixei tudo o que precisam para vir me encontrar.\n\n" +
                        "Todos os dias espero ouvir uma batida na porta da caverna, e ficaremos juntos novamente.\n\n" +
                        "<b>- John</b>";
                }
                else
                {
                    readingPanelContent = "";
                }
            }

            // Taste scene document
            if (sceneName.Contains("Taste"))
            {
                GameObject bookObj = GameObject.Find("CookbookPage");
                if (bookObj != null && bookObj.activeInHierarchy)
                {
                    readingPanelContent = "Este prato simples, porém substancioso, é feito com ingredientes do dia a dia que você provavelmente já tem na geladeira!\n\n" +
                        "<b>Você vai precisar de:</b>\n" +
                        "  - <b>Cenoura</b>\n" +
                        "  - <b>Peixe</b>\n" +
                        "  - <b>Leite de Besouro</b>\n\n" +
                        "Refogue o peixe e a cenoura no leite de besouro sobre fogo brando até cozinhar. Sirva e bom apetite!\n\n" +
                        "<color=#b8b0a2><i>(O autor e o editor não se responsabilizam por quaisquer infecções parasitárias ou virais decorrentes do consumo de frutos do mar mal cozidos)</i></color>";
                }
                else
                {
                    readingPanelContent = "";
                }
            }

            // Clear reading panel if exiting document scenes
            if (!sceneName.Contains("Taste") && !sceneName.Contains("DeadBeetle"))
            {
                readingPanelContent = "";
            }
        }

        private void OnGUI()
        {
            InitStyles();

            // Render side reading panel
            if (!string.IsNullOrEmpty(readingPanelContent))
            {
                int dynamicFontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.024f), 17, 22);
                bodyStyle.fontSize = dynamicFontSize;

                float panelWidth = Mathf.Clamp(Screen.width * 0.35f, 380f, 520f);
                float panelHeight = Mathf.Min(Screen.height * 0.86f, 620f);
                float panelX = Screen.width - panelWidth - 24f;
                float panelY = (Screen.height - panelHeight) * 0.5f;
                Rect panelRect = new Rect(panelX, panelY, panelWidth, panelHeight);

                GUI.Box(panelRect, "", panelStyle);
                DrawRectOutline(panelRect, new Color(0.72f, 0.60f, 0.38f, 0.45f), 1.5f);

                float padX = 26f;
                float padY = 24f;
                Rect contentArea = new Rect(panelRect.x + padX, panelRect.y + padY, panelRect.width - padX * 2, panelRect.height - padY * 2);

                GUILayout.BeginArea(contentArea);
                GUILayout.Label(readingPanelContent, bodyStyle);
                GUILayout.EndArea();
            }
        }

        private void DrawRectOutline(Rect rect, Color color, float thickness)
        {
            Color prevColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), dividerTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), dividerTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), dividerTexture);
            GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), dividerTexture);
            GUI.color = prevColor;
        }

        private void InitStyles()
        {
            if (panelStyle != null) return;

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = panelBgTexture;
            panelStyle.padding = new RectOffset(16, 16, 16, 16);

            bodyStyle = new GUIStyle(GUI.skin.label);
            bodyStyle.normal.textColor = new Color(0.95f, 0.93f, 0.90f, 1f);
            bodyStyle.fontSize = 19;
            bodyStyle.wordWrap = true;
            bodyStyle.richText = true;
        }
    }
}
