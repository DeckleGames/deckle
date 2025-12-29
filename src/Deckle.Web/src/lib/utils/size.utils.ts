const MM_PER_INCH = 25.4;

export function mmToPx(mm:number, dpi:number) {
    return Math.round((mm / MM_PER_INCH) * dpi);
}