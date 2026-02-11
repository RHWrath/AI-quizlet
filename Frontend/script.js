const API_BASE_URL = "http://localhost:5090";
let USERNAME = "";

let currentScore = 200;
let lives = 3;
let currentPostData = null;
let gameQueue = [];
let isProcessing = false;

const ui = {
  score: document.getElementById("score-display"),
  image: document.getElementById("game-image"),
  caption: document.getElementById("caption-text"),
  lives: document.getElementById("lives-container"),
  card: document.querySelector(".post-card"),
  footer: document.getElementById("result-footer"),

  startScreen: document.getElementById("start-screen"),
  gameScreen: document.getElementById("game-screen"),
  scoreboardScreen: document.getElementById("scoreboard-screen"),

  nameInput: document.getElementById("player-name-input"),
  startBtn: document.getElementById("start-btn"),

  // scoreboard ui
  scoreboardName: document.getElementById("scoreboard-player-name"),
  scoreboardScore: document.getElementById("scoreboard-final-score"),
  leaderboardBody: document.getElementById("leaderboard-body"),
  playAgainBtn: document.getElementById("play-again-btn"),
};

lucide.createIcons();

ui.startBtn.addEventListener("click", async () => {
  const enteredName = ui.nameInput.value.trim();

  if (!enteredName) {
    alert("Enter a player name to continue");
    return;
  }

  ui.startBtn.innerText = "CONNECTING...";
  ui.startBtn.disabled = true;

  try {
    const url = `${API_BASE_URL}/insta/createacount?name=${encodeURIComponent(enteredName)}`;

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) throw new Error("Could not create account");

    USERNAME = enteredName;

    ui.startScreen.classList.add("hidden");
    ui.gameScreen.classList.remove("hidden");

    lucide.createIcons();

    initGame();
  } catch (error) {
    console.error("Account creation error:", error);
    alert("Error connecting to server.");
    ui.startBtn.innerText = "START GAME";
    ui.startBtn.disabled = false;
  }
});

async function initGame() {
  ui.caption.innerText = "Loading game data...";
  await fetchAllPosts();
  if (gameQueue.length > 0) {
    loadNextFromQueue();
  } else {
    ui.caption.innerText = "No images found.";
  }
}

async function fetchAllPosts() {
  try {
    const response = await fetch(`${API_BASE_URL}/insta`);
    const data = await response.json();

    console.log("Data received from GET /insta:", data);

    if (Array.isArray(data)) {
      gameQueue = data;
    } else {
      gameQueue = [data];
    }
  } catch (error) {
    console.error("Error fetching posts:", error);
    ui.caption.innerText = "Error connecting to server.";
  }
}

async function validateGuessWithBackend(postId, userGuessedAi) {
  try {
    const url = `${API_BASE_URL}/insta?playerId=${USERNAME}&postId=${postId}&answer=${userGuessedAi}`;

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) throw new Error("API Error");

    const isGuessCorrect = await response.json();

    return isGuessCorrect;
  } catch (error) {
    console.error("Failed to validate guess:", error);
    return false;
  }
}

function loadNextFromQueue() {
  isProcessing = false;
  ui.card.classList.remove("glow-correct", "glow-wrong");
  ui.footer.classList.add("hidden");
  ui.footer.classList.remove("text-correct", "text-wrong");

  if (gameQueue.length === 0) {
    endGame("finished");
    return;
  }
  

  const apiPost = gameQueue.shift();

  currentPostData = {
    id: apiPost.id,
    imageUrl: apiPost.image.url,
  };

  ui.image.src = currentPostData.imageUrl;
  ui.caption.innerText = "Is this image Real or AI generated?";
  lucide.createIcons();
}

async function handleGuess(guessInput) {
  if (isProcessing || !currentPostData) return;
  isProcessing = true;

  const userGuessedAi = guessInput === "ai";

  const isCorrect = await validateGuessWithBackend(
    currentPostData.id,
    userGuessedAi,
  );

  const actuallyAi =
    (userGuessedAi && isCorrect) || (!userGuessedAi && !isCorrect);

  if (isCorrect) {
    currentScore += 50;
  } else {
    loseLife();
  }
  ui.score.innerText = currentScore;

  updateResultUI(isCorrect, actuallyAi);

  setTimeout(() => {
    loadNextFromQueue();
  }, 1500);
}

function updateResultUI(isCorrect, actuallyAi) {
  const actualLabel = actuallyAi ? "AI GENERATED" : "REAL POST";
  const iconName = isCorrect ? "check" : "x";

  ui.card.classList.add(isCorrect ? "glow-correct" : "glow-wrong");

  ui.footer.innerHTML = `<i data-lucide="${iconName}"></i> ${actualLabel}`;
  ui.footer.classList.remove("hidden");

  ui.footer.classList.remove("text-correct", "text-wrong");
  ui.footer.classList.add(isCorrect ? "text-correct" : "text-wrong");

  lucide.createIcons();
}

function loseLife() {
  if (lives > 0) {
    lives--;
    const hearts = ui.lives.querySelectorAll(".heart-icon");
    if (hearts[lives]) {
      hearts[lives].classList.add("heart-lost");
      hearts[lives].style.fill = "none";
    }
  }

  if (lives === 0) {
    setTimeout(() => {
      alert("GAME OVER - Final Score: " + currentScore);
      location.reload();
    }, 500);
  }
}

document.addEventListener("keydown", (e) => {
  if (isProcessing) return;
  if (e.key === "ArrowLeft") handleGuess("real");
  else if (e.key === "ArrowRight") handleGuess("ai");
});

//SCOREBOARD

async function submitScoreToBackend(playerName, score) {
  try {
    const url = `${API_BASE_URL}/insta/score?playerId=${encodeURIComponent(playerName)}&score=${score}`;

    const res = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
    });

    if (!res.ok) throw new Error("Failed to submit score");
    return true;
  } catch (err) {
    console.error("submitScoreToBackend error:", err);
    return false;
  }
}

async function fetchLeaderboardFromBackend() {
  try {
    const res = await fetch(`${API_BASE_URL}/insta/leaderboard`);
    if (!res.ok) throw new Error("Failed to load leaderboard");

    const data = await res.json();
    return Array.isArray(data) ? data : [];
  } catch (err) {
    console.error("fetchLeaderboardFromBackend error:", err);
    return [];
  }
}

function renderLeaderboardRows(entries) {
  ui.leaderboardBody.innerHTML = "";

  if (!entries || entries.length === 0) {
    ui.leaderboardBody.innerHTML = `
      <div class="scoreboard-row">
        <div class="col-rank">-</div>
        <div class="col-name">No scores yet</div>
        <div class="col-score">-</div>
      </div>
    `;
    return;
  }

  entries.slice(0, 10).forEach((entry, idx) => {
    const name = entry.name ?? entry.playerName ?? "Unknown";
    const score = entry.score ?? 0;

    const row = document.createElement("div");
    row.className = "scoreboard-row";
    row.innerHTML = `
      <div class="col-rank">${idx + 1}</div>
      <div class="col-name">${name}</div>
      <div class="col-score">${score}</div>
    `;
    ui.leaderboardBody.appendChild(row);
  });
}

//MOCK DATA FOR LEADERBOARD

async function endGame(reason = "") {
  isProcessing = true;

  ui.gameScreen.classList.add("hidden");
  ui.scoreboardScreen.classList.remove("hidden");

  ui.scoreboardName.innerText = USERNAME || "PLAYER";
  ui.scoreboardScore.innerText = currentScore;

  // For now: mock leaderboard
  renderLeaderboardRows([
    { name: "Name", score: 5555 },
    { name: "Name", score: 1222 },
    { name: "Name", score: 1111 },
    { name: "Name", score: 1111 },
    { name: "Name", score: 1111 },
  ]);
}



// RESET THE GAME BUTTON 

function resetGame() {
  // reset state
  currentScore = 200;
  lives = 3;
  currentPostData = null;
  gameQueue = [];
  isProcessing = false;

  // reset UI text
  if (ui.score) ui.score.innerText = currentScore;
  if (ui.caption) ui.caption.innerText = "Loading content...";
  if (ui.image) ui.image.src = "https://via.placeholder.com/400";

  // reset hearts
  if (ui.lives) {
    const hearts = ui.lives.querySelectorAll(".heart-icon");
    hearts.forEach((h) => {
      h.classList.remove("heart-lost");
      h.style.fill = ""; // back to CSS default
    });
  }

  // reset result footer/card glow
  if (ui.card) ui.card.classList.remove("glow-correct", "glow-wrong");
  if (ui.footer) {
    ui.footer.classList.add("hidden");
    ui.footer.classList.remove("text-correct", "text-wrong");
    ui.footer.innerHTML = "";
  }

  // show start screen, hide others
  ui.scoreboardScreen?.classList.add("hidden");
  ui.gameScreen?.classList.add("hidden");
  ui.startScreen?.classList.remove("hidden");

  // reset start button
  if (ui.startBtn) {
    ui.startBtn.disabled = false;
    ui.startBtn.innerText = "START GAME";
  }

  // optionally clear name field
  if (ui.nameInput) ui.nameInput.value = "";

  lucide.createIcons();
}

ui.playAgainBtn.addEventListener("click", () => {
  resetGame();
});
