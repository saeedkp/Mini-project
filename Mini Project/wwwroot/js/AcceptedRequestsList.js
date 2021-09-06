const targetDiv1 = document.getElementById("not-checked");
const targetDiv2 = document.getElementById("need-correction");
const notCheckedBtn = document.getElementById("not-checked-button");
const needCorrectionBtn = document.getElementById("need-correction-button");

notCheckedBtn.onclick = function () {
    targetDiv2.style.display = "none";
    targetDiv1.style.display = "block";
};

needCorrectionBtn.onclick = function () {
    targetDiv1.style.display = "none";
    targetDiv2.style.display = "block";
};