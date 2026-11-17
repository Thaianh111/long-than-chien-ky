/** Khung nhìn: canvas 2D cao VH px, rộng VW thay đổi theo tỉ lệ màn hình. */
export interface View { cx: CanvasRenderingContext2D; VW: number; cam: { x: number; y: number }; alpha: number }
