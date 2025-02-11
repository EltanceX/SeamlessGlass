// @2025 EltanceX
// annularwind@outlook.com
using Engine.Graphics;
using Engine;

namespace Game
{
    public abstract class ECubeBlock : AlphaTestCubeBlock
    {
        public Texture2D texture2D;
        public Texture2D handTexture;
        public ECubeBlock(string TexturePath)
        {
            FirstPersonOffset = new Vector3(0.5f, -0.5f, -0.6f);
            FirstPersonRotation = new Vector3(0, 40, 0);
            FirstPersonScale = 0.2f;
            InHandScale = 0.5f;
            InHandOffset = new Vector3(0, 0.1f, -0.2f);
            this.texture2D = ContentManager.Get<Texture2D>(TexturePath);

        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            color = Color.White;
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), 1, ref matrix, color, color, environmentData, handTexture ?? texture2D);
            //DevicesBlockManager.GetDevice(value).DrawBlock(primitivesRenderer,value,color,size,ref matrix,environmentData);

            //base.DrawBlock(primitivesRenderer,value,color,size,ref matrix,environmentData);
            //BlocksManager.DrawFlatBlock(primitivesRenderer,value,size,ref matrix,flatTexture,Color.White,true,environmentData);
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            //base.GenerateTerrainVertices(generator,geometry,value,x,y,z);
            generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.GetGeometry(texture2D).AlphaTestSubsetsByFace);
        }
        public override int GetTextureSlotCount(int value)
        {
            return 1;
        }
        //public override get
        public override int GetFaceTextureSlot(int face, int value)
        {
            switch (face)
            {
                case 0: return 0; //z+
                case 1: return 1; //x+
                case 2: return 2; //z-
                case 3: return 3; //x-
                case 4: return 4; //y+
                case 5: return 5; //y-
            }
            return 1;
        }
        public override bool GenerateFacesForSameNeighbors_(int value)
        {
            return base.GenerateFacesForSameNeighbors_(value);
        }
    }
    public class SeamlessGlass : ECubeBlock
    {
        public enum DTFace
        {
            YP = 5,
            YN = 4,
            XP = 3,
            XN = 2,
            ZP = 1,
            ZN = 0,
        }
        public const int Index = 910;

        public SeamlessGlass() : base("Textures/Blocks2")
        {
            DefaultDisplayName = "SeamlessGlass";
            DefaultDescription = "Seamless connected glass";
            DefaultCategory = "Construction";
            CraftingId = "SeamlessGlass";
            DefaultDropContent = Index;
            Behaviors = typeof(TestCubeBehavior).Name;
            this.IsTransparent = true;
            RequiredToolLevel = 0;
            DefaultSoundMaterialName = "Glass";
            this.handTexture = ContentManager.Get<Texture2D>("Textures/SeamlessGlass");
            IsDiggingTransparent = false;
            MaxStacking = 100;
            DigMethod = BlockDigMethod.Quarry;
            DigResilience = 1;
            DefaultDropContent = BlocksManager.GetBlockIndex<SeamlessGlass>();
        }
        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            //return base.CreateDebrisParticleSystem(subsystemTerrain,position,value,strength);
            //int data = Terrain.ExtractData(value);
            //Color fabricColor = SubsystemPalette.GetFabricColor(subsystemTerrain,GetColor(data));

            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, Color.White, DefaultTextureSlot, handTexture);
        }
        public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel, float playerLevel)
        {
            return base.GetAdHocCraftingRecipe(subsystemTerrain, ingredients, heatLevel, playerLevel);
        }
        public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
        {
            //return base.GetProceduralCraftingRecipes();
            var craftingRecipe = new CraftingRecipe
            {
                ResultCount = 2,
                //ResultValue = Terrain.MakeBlockValue(182,0,SetColor(0,color)),
                ResultValue = BlocksManager.GetBlockIndex<SeamlessGlass>(),
                RemainsCount = 0,
                //RemainsValue = Terrain.MakeBlockValue(90),
                RequiredHeatLevel = 0f,
                Description = "无缝连接的玻璃"//LanguageControl.Get(GetType().Name,1)
            };
            craftingRecipe.Ingredients[0] = "glass";
            craftingRecipe.Ingredients[1] = "glass";
            craftingRecipe.Ingredients[2] = "glass";
            //craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
            yield return craftingRecipe;
        }
        public static int ExtractBit(int n, int loc)
        {
            return (n >> loc) & 1;
        }
        public static int ExtractBit(int n, DTFace loc)
        {
            return ExtractBit(n, (int)loc);
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (value == 910 || value == 16270) return 0;
            int blockValue = Terrain.ExtractData(value);
            int l = 0;
            int b = 0;
            int r = 0;
            int t = 0;
            switch (face)
            {
                case 0://z+ 
                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 1://x+
                    l = ExtractBit(blockValue, DTFace.ZP) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.ZN) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 2: //z-
                    l = ExtractBit(blockValue, DTFace.XP) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.XN) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 3://x-
                    l = ExtractBit(blockValue, DTFace.ZN) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.ZP) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 4://y+

                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.ZP) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.ZN) << 0;
                    break;
                case 5://y- (同y+)
                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.ZN) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.ZP) << 0;
                    break;
            }
            return l | b | r | t;
        }
    }
    public class TestCubeBehavior : SubsystemBlockBehavior
    {
        //public override  
        public static int[][] detect = {
        new int[]{0, 1, 0 , 0b100000}, 
		new int[] { 0,-1,0, 0b010000 },
		new int[] { 1,0,0,0b001000 },  
		new int[]{-1, 0, 0, 0b000100 },
		new int[] {0, 0, 1,0b000010 }, 
		new int[]{ 0, 0, -1,0b000001 } 
	};
        public int UpdateConnectValue(int x, int y, int z)
        {
            var terrain = SubsystemTerrain.Terrain;
            int value = 0;

            foreach (int[] toDetect in detect)
            {
                int cell = terrain.GetCellValue(x + toDetect[0], y + toDetect[1], z + toDetect[2]);
                int content = Terrain.ExtractContents(cell);
                var block = BlocksManager.Blocks[content];
                if (block is SeamlessGlass)
                {
                    value |= toDetect[3];
                }
            }
            return value;
        }
        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            base.OnBlockAdded(value, oldValue, x, y, z);
            var terrain = SubsystemTerrain.Terrain;
            var a = terrain.GetCellValue(x, y, z);
            var a2 = Terrain.ReplaceData(value, 0b000001);
            var b = terrain.GetCellContentsFast(x, y, z);
            SubsystemTerrain.ChangeCell(x, y, z, a2);
        }
        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
            int newValue = UpdateConnectValue(x, y, z);
            var terrain = SubsystemTerrain.Terrain;
            var v = terrain.GetCellValue(x, y, z);
            var a = Terrain.ExtractData(v);
            var a2 = Terrain.ReplaceData(v, newValue);
            if (a != a2) SubsystemTerrain.ChangeCell(x, y, z, a2);
        }
    }
}
