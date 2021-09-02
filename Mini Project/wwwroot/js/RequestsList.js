const targetDiv1 = document.getElementById("not-checked");
const targetDiv2 = document.getElementById("rejected");
const notCheckedBtn = document.getElementById("not-checked-button");
const rejectedBtn =  document.getElementById("rejected-button");

notCheckedBtn.onclick = function () {
    targetDiv2.style.display = "none";
    targetDiv1.style.display = "block";
};

rejectedBtn.onclick = function () {
      targetDiv1.style.display = "none";
      targetDiv2.style.display = "block";
  };