namespace NGDtuanh.Utils {
    public struct Padding {
        public float left;
        public float right;
        public float top;
        public float bot;

        public float horizontal => left + right;
        public float vertical   => top  + bot;

        public Padding(float left, float right, float top, float bot) {
            this.left  = left;
            this.right = right;
            this.top   = top;
            this.bot   = bot;
        }

        public Padding With_Left(float left) {
            var result = this;
            result.left = left;
            return result;
        }
        
        public Padding With_Right(float right) {
            var result = this;
            result.right = right;
            return result;
        }
        
        public Padding With_Top(float top) {
            var result = this;
            result.top = top;
            return result;
        }
        
        public Padding With_Bot(float bot) {
            var result = this;
            result.bot = bot;
            return result;
        }
    }
}