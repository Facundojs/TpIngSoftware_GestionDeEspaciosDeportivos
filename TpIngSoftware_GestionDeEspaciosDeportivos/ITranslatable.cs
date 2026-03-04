namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    /// <summary>
    /// Implemented by UI forms and user controls that support runtime language switching.
    /// </summary>
    /// <remarks>
    /// Called by the application shell when the user changes the active language.
    /// Implementations should re-apply all localizable string resources to their controls
    /// without recreating or re-initializing the form.
    /// </remarks>
    public interface ITranslatable
    {
        /// <summary>Re-applies the current language strings to all translatable UI controls.</summary>
        void UpdateLanguage();
    }
}
