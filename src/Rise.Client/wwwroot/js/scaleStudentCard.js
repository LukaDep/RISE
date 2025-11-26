const BASE_STUDENT_CARD_WIDTH = 123.0
window.scaleStudentCard = function () {
    const element = document.getElementById('student-card')
    if (!element || !element.parentElement) {
        return
    }
    const computedStyle = window.getComputedStyle(element.parentElement)
    const parentWidth = parseFloat(computedStyle.width)
    element.style.scale = parentWidth / BASE_STUDENT_CARD_WIDTH
}

window.addEventListener('resize', window.scaleStudentCard)
