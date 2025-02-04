namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeHeaderText : PgbePanel
    {
        protected override void ResetTgtData()
        { }

        internal void OnOpen(string text)
        {
            titleLabel.text = text;
        }
    }
}