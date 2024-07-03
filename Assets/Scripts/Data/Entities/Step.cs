namespace Data.Entities
{
    [System.Serializable]
    public class Step
    {
        public int StepID;
        public int OperationID;
        public string Description;
        public int StepIndex;
        public string PickPosition;
        public string Piece;
        public float AssemblyTime;
        public string CommandVoice;
        public string ConfirmationVoice;
        public string DenialVoice;
        public float PX;
        public float PY;
        public float PZ;
        public float RX;
        public float RY;
        public float RZ;
        public float RW;
        public float SX;
        public float SY;
        public float SZ;
    }
}