-- ============================================================================
-- 02_procedures.sql
-- Genuine PostgreSQL routines that the application ACTUALLY calls:
--   * sp_insert_triage_result – stores one anonymized result (called via CALL)
--   * fn_role_statistics – average scores per role (called via SELECT)
-- The script is idempotent (CREATE OR REPLACE) and is run by the application on startup.
-- The triage_results table schema is driven by EF Core migrations (see 01_schema.sql).
-- ============================================================================

-- Insert a triage result -------------------------------------------------------
CREATE OR REPLACE PROCEDURE sp_insert_triage_result(
    p_role                        integer,
    p_early_adoption_score        integer,
    p_experimentation_score       integer,
    p_iteration_score             integer,
    p_output_verification_score   integer,
    p_saved_workflows_score       integer,
    p_sharing_score               integer,
    p_total_score                 integer,
    p_level                       integer,
    p_management                  boolean
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Plain INSERT without its own transaction control (safe inside the caller's transaction).
    INSERT INTO triage_results (
        role,
        early_adoption_score, experimentation_score, iteration_score,
        output_verification_score, saved_workflows_score, sharing_score,
        total_score, level, management
    ) VALUES (
        p_role,
        p_early_adoption_score, p_experimentation_score, p_iteration_score,
        p_output_verification_score, p_saved_workflows_score, p_sharing_score,
        p_total_score, p_level, p_management
    );
END;
$$;

-- Statistics per role ---------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_role_statistics()
RETURNS TABLE (
    role                              integer,
    response_count                    bigint,
    avg_early_adoption_score          double precision,
    avg_experimentation_score         double precision,
    avg_iteration_score               double precision,
    avg_output_verification_score     double precision,
    avg_saved_workflows_score         double precision,
    avg_sharing_score                 double precision,
    avg_total_score                   double precision
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        t.role,
        COUNT(*)                                   AS response_count,
        AVG(t.early_adoption_score)::double precision,
        AVG(t.experimentation_score)::double precision,
        AVG(t.iteration_score)::double precision,
        AVG(t.output_verification_score)::double precision,
        AVG(t.saved_workflows_score)::double precision,
        AVG(t.sharing_score)::double precision,
        AVG(t.total_score)::double precision
    FROM triage_results t
    GROUP BY t.role
    ORDER BY t.role;
$$;
