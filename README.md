# Finding-the-Way-Using-Genetic-Algorithms
Finding the Way Using Genetic Algorithms

<img width="3444" height="1966" alt="image" src="https://github.com/user-attachments/assets/940b495f-1d0a-4d5b-aff5-11416b9792e0" />


# demonstration video
https://youtu.be/HcvP1RVyj74

---

## 📖 Overview

This project simulates evolution to solve a problem: finding the correct sequence of moves to reach the end of a path. A population of agents (players), each with a unique set of instructions (their "DNA"), attempts to navigate the course.

Over successive generations, the agents learn from the successes of their ancestors. Through the core GA principles of **Selection**, **Crossover**, and **Mutation**, the population as a whole "evolves" an optimal solution to find the goal efficiently.

---

## 🧠 Core Concepts of the Genetic Algorithm

A Genetic Algorithm is a search heuristic inspired by Charles Darwin's theory of natural selection. This project implements the following key concepts:

-   **Population**: A group of individuals (our `PlayerController` agents) that each represent a potential solution.
-   **DNA (or Chromosome)**: A set of instructions that dictates an agent's behavior. In this project, it's an array of integers (`int[] DNA`) where each number (gene) corresponds to an action (e.g., turn north, stay still).
-   **Fitness**: A score that measures how "good" a solution is. Here, an agent's fitness is the `Score` it achieves by reaching tiles further along the path.
-   **Selection**: The process of choosing the fittest individuals from the current generation to be parents for the next. In this simulation, agents with a fitness score greater than or equal to the population's average are selected as "elites."
-   **Crossover**: Creating a new individual (offspring) by combining the DNA from two parents. This mimics biological reproduction, passing on successful traits.
-   **Mutation**: Introducing small, random changes into an individual's DNA. This maintains genetic diversity in the population and allows for new, potentially better, solutions to emerge.

---

---

## 💡 Ideas for Future Improvements

This project provides a solid foundation for understanding genetic algorithms. Here are several ideas on how the algorithm and simulation could be enhanced for faster convergence and more robust solutions.

### 1. Enhanced Elitism: The "Hall of Fame"

* **Current System**: The algorithm preserves the "elite" individuals from the *immediately preceding* generation.
* **Proposed Idea**: Implement a "Hall of Fame" mechanism. This involves storing the single best DNA sequence found across **all** generations. In each new generation, this "all-time champion" is guaranteed a spot, potentially replacing the weakest offspring.
* **Potential Benefit**: This prevents evolutionary regression, where a highly successful solution from an earlier generation might be accidentally lost due to random chance in crossover or mutation. It ensures the best-found solution is always preserved.
* **Potential Downside: Premature Convergence to a Local Optimum.**
    If a "good enough" but not truly optimal solution is found early, the Hall of Fame mechanism can cause the entire population to converge on this single solution too quickly. The constant presence of this champion's DNA might "overpower" the gene pool, stifling the exploration needed to find a more creative or globally optimal path that might exist.


### 2. Time-Based Generations vs. Survival-Based

* **Current System**: A new generation is triggered when the number of living agents falls below a set threshold. This is a **survival-based** model.
* **Proposed Idea**: Switch to a **time-based** model. Allow all agents to run for a fixed duration (e.g., 20 seconds). When the timer expires, the generation ends, and the fitness of every agent is evaluated based on how far they traveled.
* **Potential Benefit**: This changes the evolutionary pressure from rewarding pure survival to rewarding **efficiency**. It encourages the evolution of agents that find the goal *faster*, not just agents that avoid obstacles the longest. It also provides a more consistent duration for each generation.
* **Potential Downside: Critical Need for Hyperparameter Tuning.**
    The effectiveness of this model is highly dependent on setting the correct time limit for a generation.
    * **If the time is too short:** No agent may ever reach the goal, providing poor fitness data and making it difficult for the algorithm to learn.
    * **If the time is too long:** The simulation can become inefficient. Agents might find the goal early and then do nothing for the remaining time, wasting computational resources without providing new information.


### 3. Alternative Penalty System (Instead of "Death")

* **Current System**: An agent "dies" (is deactivated) upon hitting a wall or obstacle. This is a binary outcome.
* **Proposed Idea**: Instead of immediate death, apply a fitness penalty. For example, hitting a wall could subtract a significant amount from an agent's score or freeze it in place for a few moments.
* **Potential Benefit**: This provides more granular learning data. An agent that almost reaches the goal before hitting a wall is still "fitter" than an agent that hits a wall at the very start. A penalty system allows the algorithm to learn from these "near misses," whereas the current death model treats both failures equally.
* **Potential Downside: Complexity in Fitness Function Design.**
    The benefit of this system depends entirely on how the penalty is tuned.
    * **If the penalty is too small:** Agents may not learn to avoid obstacles effectively, as the fitness cost is negligible.
    * **If the penalty is too large:** It can become functionally identical to the original "death" system, negating the benefits of a more granular approach.
    This adds another sensitive parameter that must be carefully balanced with the scoring system, making the overall fitness landscape more complex to design and debug.


### 4. Training on Parallel, Varied Environments

* **Current System**: The entire population evolves on a single, static stage.
* **Proposed Idea**: Create several different stage layouts. In each generation, evaluate an agent's fitness based on its *average performance* across all these different environments.
* **Potential Benefit**: This forces the evolution of more **robust and generalized** solutions. An agent that can solve multiple different paths is inherently "smarter" than an agent that has simply "memorized" the optimal path for one specific layout. This approach helps prevent overfitting to a single problem.
* **Potential Downside: Significant Increase in Computational Cost.**
    Computer performance may be a problem, but the problem is not likely to be big


## ⚙️ How It Works

The simulation progresses through a cycle that repeats for each generation:

1.  **Initialization**: The simulation begins with an initial population (`PlayerCount`) of agents. Each agent is given a completely random sequence of instructions (DNA).

2.  **Evaluation**: All agents attempt to navigate the path simultaneously. Their actions at any given moment are dictated by the current gene in their DNA sequence.
    -   If an agent collides with a wall or obstacle, it "dies" and is removed from the current simulation round.
    -   As an agent successfully travels across floor tiles, its **Fitness Score** increases.

3.  **Selection**: When the number of living agents drops below a certain threshold, the generation ends. The simulation pauses, and the "fittest" agents (the survivors with above-average scores) are selected as parents for the next generation.

4.  **Reproduction (Crossover & Mutation)**: A new population is created.
    -   The "elite" survivors are carried over directly to the next generation, preserving the best solutions found so far.
    -   The remaining population slots are filled with new "offspring." Each offspring is created by taking two elite parents at random, combining their DNA (**Crossover**), and applying a few random tweaks (**Mutation**).

5.  **Evolution**: The new generation, now containing a mix of proven elites and promising offspring, is reset to the starting line. The cycle begins again from Step 2.

This loop continues, and with each generation, the average fitness of the population increases until an agent finally reaches the target score, successfully solving the path.

---

## 🛠️ Key Scripts

-   `GameManager.cs`: The main controller for the simulation. It manages the population, orchestrates the generational loop, and holds key parameters for the simulation (population size, mutation rate, etc.).
-   `PlayerController.cs`: Represents a single agent in the population. It handles movement, collision detection (for scoring and death), and holds the agent's individual DNA and fitness score.
-   `GeneticAlgorithm.cs`: A non-Unity, data-oriented class that contains the core logic for the GA. It handles the selection, crossover, and mutation operations, taking a population's data and returning a new generation of DNA.

---

## 🚀 How to Use

1.  Clone this repository.
2.  Open the project in the Unity Hub.
3.  Open the main scene file (e.g., `MainScene.unity`).
4.  Select the `GameManager` object in the Hierarchy.
5.  Adjust the public parameters in the Inspector to configure the simulation.
6.  Press **Play**.

### Configurable Parameters (on `GameManager`)

-   **DNA Settings**:
    -   `Dna Size`: The total number of instructions (genes) for each agent.
    -   `Gene Size`: The size of a gene block for crossover operations.
    -   `Mutation Size`: The maximum number of genes that can be randomly changed during mutation.
-   **Population Settings**:
    -   `Player Count`: The total number of agents in each generation.
    -   `Reset Count`: The simulation will proceed to the next generation when the number of living players falls to this number.
-   **Player Settings**:
    -   `Action Interval`: The time (in seconds) each agent spends performing one action from its DNA.
    -   `Player Speed`: The movement speed of the agents.

