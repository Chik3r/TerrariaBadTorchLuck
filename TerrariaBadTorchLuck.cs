using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaBadTorchLuck
{
	public class TerrariaBadTorchLuck : Mod
	{
		public override void Load()
		{
			IL.Terraria.Player.UpdateTorchLuck_ConsumeCountersAndCalculate += ILTorchLuckClamp;
		}

		private void ILTorchLuckClamp(ILContext il)
		{
			/*
			 * IL_041F: ldarg.0
			 * IL_0420: ldfld     float32 Terraria.Player::torchLuck
			 * IL_0425: ldc.r4    0.0
			 * IL_042A: clt
			 * IL_042C: stloc.s   V_34
			 * IL_042E: ldloc.s   V_34
			 * <======>
			 * INSERT:  pop
			 * INSERT:  br.s      IL_043D
			 * <======>
			 * IL_0430: brfalse.s IL_043D
			 */

			ILCursor c = new(il);
			ILLabel targetLabel = null;

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld(typeof(Player).GetField("torchLuck")),
				i => i.MatchLdcR4(0),
				i => i.MatchClt(),
				i => i.MatchStloc(34),
				i => i.MatchLdloc(34),
				i => i.MatchBrfalse(out targetLabel))) {
				Logger.Warn("Failed to apply IL edit to UpdateTorchLuck_ConsumeCountersAndCalculate");
				return;
			}

			c.Index--;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Br_S, targetLabel);
		}
	}
}