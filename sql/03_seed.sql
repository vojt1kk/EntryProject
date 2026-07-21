-- ============================================================================
-- 03_seed.sql
-- Test (anonymized) data: 150 results, 30 per each of the 5 roles.
-- Habit scores are 0–100, total score = average of the 6 habits, level per the
-- thresholds (0–39 / 40–74 / 75–100). Different roles have different averages
-- so the statistics are interesting. The script is idempotent: it inserts
-- nothing if the table already contains data.
-- The application additionally runs it only when the table is empty.
-- ============================================================================

INSERT INTO triage_results (
    role,
    early_adoption_score, experimentation_score, iteration_score,
    output_verification_score, saved_workflows_score, sharing_score,
    total_score, level, management, created_at
)
SELECT
    t.role,
    t.s1, t.s2, t.s3, t.s4, t.s5, t.s6,
    t.total,
    CASE WHEN t.total < 40 THEN 1 WHEN t.total < 75 THEN 2 ELSE 3 END AS level,
    CASE WHEN t.role = 5 AND t.idx % 4 = 0 THEN true ELSE false END AS management,
    now() - ((t.idx * 7 + t.role) || ' hours')::interval AS created_at
FROM (
    SELECT
        s.role, s.idx,
        s.s1, s.s2, s.s3, s.s4, s.s5, s.s6,
        round((s.s1 + s.s2 + s.s3 + s.s4 + s.s5 + s.s6) / 6.0)::int AS total
    FROM (
        SELECT
            rb.role, g AS idx,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s1,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s2,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s3,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s4,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s5,
            LEAST(100, GREATEST(0, rb.base + (floor(random() * 41)::int - 20))) AS s6
        FROM (VALUES (1, 32), (2, 55), (3, 72), (4, 50), (5, 46)) AS rb(role, base)
        CROSS JOIN generate_series(1, 30) AS g
    ) s
) t
WHERE NOT EXISTS (SELECT 1 FROM triage_results);
