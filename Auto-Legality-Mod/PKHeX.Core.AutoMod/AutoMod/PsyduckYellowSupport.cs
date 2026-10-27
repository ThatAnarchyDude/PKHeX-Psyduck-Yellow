using System;
using System.Linq;
namespace PKHeX.Core.AutoMod;

/// <summary>
/// Psyduck Yellow generation: Pokémon are built from the game's own encounters (the Pallet starter gifts and the Kanto wild areas).
/// The starter Psyduck/Eevee line is recognised by its own species IDs, never by guessing from a plain Psyduck or Eevee.
/// </summary>
public static class PsyduckYellowSupport
{
    private const int MaxAttempts = 50;

    /// <summary>Kept for the shared ALM call sites; Psyduck Yellow saves need no profile flag on the template.</summary>
    public static void AttachProfile(ITrainerInfo trainer, PKM template)
    {
        if (trainer is SAV4PY && template is PK4 pk)
        {
            pk.IsPsyduckYellow0029 = true;
            pk.IsExpandedEngine = true;
        }
    }

    public static bool TryGenerate(ITrainerInfo trainer, PKM template, IBattleTemplate set,
        out PKM result, out LegalizationResult status)
    {
        result = template;
        status = LegalizationResult.Failed;
        if (trainer is not SAV4PY || template is not PK4)
            return false;

        // A request this generator can't honour fails explicitly instead of being silently dropped.
        var species = set.Species;
        // The ALM's tables only know the ordinary species: work on the ordinary-species view of the request, then put the game's own species back.
        if (PsyduckYellowExpanded.GetRetailSpecies(species) != species)
        {
            if (template.Species != species)
                return true; // the request names a species that only exists in this game: it must come from the Pokémon itself
            var view = template.Clone();
            view.Species = PsyduckYellowExpanded.GetRetailSpecies(species);
            set = new RegenTemplate(new ShowdownSet(view), 4);
        }
        var regen = (set as RegenTemplate)?.Regen;
        if (set.Form != 0 || set.Level < 1 || set.Level > 100 || set.CanGigantamax ||
            regen?.Extra is { Egg: true } or { Alpha: true })
            return true;

        var isStarterLine = PsyduckYellowExpanded.IsStarterSpecies(species);
        var criteria = EncounterCriteria.GetCriteria(set, template.PersonalInfo, EncounterMutationUtil.GetSuggested(trainer.Context, set.Level));
        foreach (var enc in PsyduckYellowEncounterSearch.Search(species))
        {
            // Wild Psyduck/Eevee are ordinary species; the starter line only ever comes from the Pallet gift.
            var isStarterGift = enc is EncounterStatic4 { IsPsyduckYellowStarter: true };
            var gift = isStarterGift && enc.Species != (ushort)Species.Togepi;
            if (gift != isStarterLine && (gift || isStarterLine))
                continue;
            if (enc.LevelMin > set.Level)
                continue;
            for (int i = 0; i < MaxAttempts; i++)
            {
                if (enc.ConvertToPKM(trainer, criteria) is not PK4 pk)
                    break;
                pk.Species = set.Species; // ordinary species while the ALM edits it
                pk.CurrentLevel = (byte)Math.Max(set.Level, pk.MetLevel);
                APILegality.ApplySetDetails(pk, set, trainer, enc, regen ?? RegenSet.Default, criteria);
                PsyduckYellowEncounterSearch.AdaptToGame(pk, enc);
                pk.Species = species;
                pk.Nickname = PsyduckYellowExpanded.SpeciesNames[species];
                pk.HealPP();
                pk.ResetPartyStats();
                pk.RefreshChecksum();
                var la = new LegalityAnalysis(pk);
                if (!la.Valid)
                    continue;
                if (set.Shiny != pk.IsShiny && i < MaxAttempts - 1)
                    continue;
                result = pk;
                status = LegalizationResult.Regenerated;
                return true;
            }
        }
        return true;
    }
}
