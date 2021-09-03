const targetDiv1 = document.getElementById("setted-interviews");
const targetDiv2 = document.getElementById("ended-interviews");
const targetDiv3 = document.getElementById("rejected");
const targetDiv4 = document.getElementById("accepted");

const settedInterviewsdBtn = document.getElementById("setted-interviews-button");
const endedInterviewsBtn = document.getElementById("ended-interviews-button");
const rejectedBtn =  document.getElementById("rejected-button");
const acceptedBtn =  document.getElementById("accepted-button");

settedInterviewsdBtn.onclick = function () {
    targetDiv2.style.display = "none";
    targetDiv3.style.display = "none";
    targetDiv4.style.display = "none";
    targetDiv1.style.display = "block";
};
endedInterviewsBtn.onclick = function () {
    targetDiv1.style.display = "none";
    targetDiv3.style.display = "none";
    targetDiv4.style.display = "none";
    targetDiv2.style.display = "block";
};

rejectedBtn.onclick = function () {
    targetDiv1.style.display = "none";
    targetDiv2.style.display = "none";
    targetDiv4.style.display = "none";
    targetDiv3.style.display = "block";
};

acceptedBtn.onclick = function () {
    targetDiv1.style.display = "none";
    targetDiv2.style.display = "none";
    targetDiv3.style.display = "none";
    targetDiv4.style.display = "block";
};