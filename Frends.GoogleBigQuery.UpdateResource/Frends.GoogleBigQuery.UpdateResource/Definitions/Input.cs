using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.UpdateResource.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Resource type.
    /// </summary>
    /// <example>Resources.Dataset</example>
    [DefaultValue(Resources.Dataset)]
    public Resources Resource { get; set; }

    /// <summary>
    /// Resource description.
    /// </summary>
    /// <example>Example description.</example>
    public string Description { get; set; }

    /// <summary>
    /// Friendly name for Dataset or Table resource.
    /// </summary>
    /// <example>Example name</example>
    public string FriendlyName { get; set; }

    /// <summary>
    /// Label(s) for Dataset or Table resource. (Optional)
    /// Label keys and values can be no longer than 63 characters, can only contain lowercase letters, numeric characters, underscores and dashes. 
    /// International characters are allowed. 
    /// Label keys must start with a letter and each label in the list must have a different key.
    /// </summary>
    /// <example>[ {foo, bar }, { bar, foo } ]</example>
    public LabelParameters[] Label { get; set; }

    /// <summary>
    /// Set Dataset or Table encryption configuration.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool SetEncryptionConfiguration { get; set; }

    /// <summary>
    /// Describes the Cloud KMS encryption key that will be used to protect destination BigQuery table.
    /// The BigQuery Service Account associated with your project requires access to this encryption key.
    /// </summary>
    /// <example>projects/your-project/locations/global/keyRings/your-key-ring/cryptoKeys/your-key</example>
    [UIHint(nameof(SetEncryptionConfiguration), "", true)]
    public string KmsKeyName { get; set; }

    #region Dataset specific

    /// <summary>
    /// The geographic location where the dataset should reside. 
    /// See details at https://cloud.google.com/bigquery/docs/locations
    /// </summary>
    /// <example>europe-north1</example>
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("EU")]
    public string Location { get; set; }

    /// <summary>
    /// Storage billing model to be used for all tables in the dataset.
    /// Can be set to PHYSICAL. 
    /// </summary>
    /// <example>LOGICAL</example>
    [DefaultValue("LOGICAL")]
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    public string StorageBillingModel { get; set; }

    /// <summary>
    /// Indicates if table names are case insensitive in the dataset.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    [DefaultValue(false)]
    public bool IsCaseInsensitive { get; set; }

    /// <summary>
    /// Number of hours for the max time travel for all tables in the dataset.
    /// Unlimited if 0.
    /// </summary>
    /// <example>0</example>
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    [DefaultValue(0)]
    public long MaxTimeTravelHours { get; set; }

    /// <summary>
    /// An array of objects that define dataset access for one or more entities. (Optional)
    /// You can set this property when inserting or updating a dataset in order to control who is allowed to access the data. 
    /// If unspecified at dataset creation time, BigQuery adds default dataset access for the following entities:
    /// access.specialGroup: projectReaders; access.role: READER; access.specialGroup: projectWriters; access.role: WRITER; access.specialGroup: projectOwners; access.role: OWNER; access.userByEmail: [dataset creator email]; access.role: OWNER;
    /// </summary>
    /// <example>[ { READER, foo@bar.com }, { WRITER, bar@bar.com } ]</example>
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    public AccessParameters[] Access { get; set; }

    /// <summary>
    /// The tags associated with this dataset. (Optional)
    /// Tag keys are globally unique.
    /// </summary>
    /// <example>[ { foo, bar }, { foo2, bar2 } ]</example>
    [UIHint(nameof(Resource), "", Resources.Dataset)]
    public TagParameters[] Tag { get; set; }
    #endregion Dataset specific

    #region Routine specific
    /// <summary>
    /// The body of the routine. 
    /// For functions, this is the expression in the AS clause. 
    /// If language=SQL, it is the substring inside (but excluding) the parentheses. 
    /// For example, for the function created with the following statement: 
    /// `CREATE FUNCTION JoinLines(x string, y string) as (concat(x, "\n", y))` The definition_body is `concat(x, "\n", y)` (\n is not replaced with linebreak). 
    /// If language=JAVASCRIPT, it is the evaluated string in the AS clause. 
    /// For example, for the function created with the following statement:
    /// `CREATE FUNCTION f() RETURNS STRING LANGUAGE js AS 'return "\n";\n'` The definition_body is `return "\n";\n`
    /// Note that both \n are replaced with linebreaks.
    /// </summary>
    /// <example>concat(x, \"\\n\", y)</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public string DefinitionBody { get; set; }

    /// <summary>
    /// Defaults to "SQL" if Input.RemoteFunctionParameters is not empty, not set otherwise. (Optional)
    /// </summary>
    /// <example>SQL</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    [DisplayFormat(DataFormatString = "Text")]
    public string Language { get; set; }

    /// <summary>
    /// Routine type.
    /// </summary>
    /// <example>RoutineTypes.SCALAR_FUNCTION</example>
    [DefaultValue(RoutineTypes.SCALAR_FUNCTION)]
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public RoutineTypes RoutineType { get; set; }

    /// <summary>
    /// Optional if language = "SQL"; required otherwise. 
    /// Cannot be set if Input.RoutineType = "TABLE_VALUED_FUNCTION".
    /// If absent, the return type is inferred from definition_body at query time in each query that references this routine. 
    /// If present, then the evaluated result will be cast to the specified returned type at query time.
    /// For example, for the functions created with the following statements: * `CREATE FUNCTION Add(x FLOAT64, y FLOAT64) RETURNS FLOAT64 AS (x + y);` * `CREATE FUNCTION Increment(x FLOAT64) AS (Add(x, 1));` * `CREATE FUNCTION Decrement(x FLOAT64) RETURNS FLOAT64 AS (Add(x, -1));` The return_type is `{type_kind: "FLOAT64"}` for `Add` and `Decrement`, and is absent for `Increment` (inferred as FLOAT64 at query time). 
    /// Suppose the function `Add` is replaced by `CREATE OR REPLACE FUNCTION Add(x INT64, y INT64) AS (x + y);` Then the inferred return type of `Increment` is automatically changed to INT64 at query time, while the return type of `Decrement` remains FLOAT64.
    /// </summary>
    /// <example>DATE</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public string ReturnType { get; set; }

    /// <summary>
    /// The determinism level of the JavaScript UDF, if defined.
    /// </summary>
    /// <example>DeterminismLevels.DETERMINISTIC</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    [DefaultValue(DeterminismLevels.DETERMINISM_LEVEL_UNSPECIFIED)]
    public DeterminismLevels DeterminismLevel { get; set; }

    /// <summary>
    /// Can be set for procedures only. 
    /// If true (default), the definition body will be validated in the creation and the updates of the procedure. 
    /// For procedures with an argument of ANY TYPE, the definition body validtion is not supported at creation/update time, and thus this field must be set to false explicitly.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    [DefaultValue(true)]
    public bool StrictMode { get; set; }

    /// <summary>
    /// If Input.Language = "JAVASCRIPT", this field stores the path of the imported JAVASCRIPT libraries. (Optional)
    /// </summary>
    /// <example>[ { foo }, { bar } ]</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public ImportedLibraryParameters[] ImportedLibrary { get; set; }

    /// <summary>
    /// Argument(s) of a function or a stored procedure.
    /// </summary>
    /// <example>[ { x, IN, INT64, FIXED_TYPE }, { y, OUT, INT64, FIXED_TYPE }, { 'not set', 'not set', INT64, 'not set' } ]</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public ArgumentParameters[] Argument { get; set; }

    /// <summary>
    /// Set remote function specific options.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    [DefaultValue(false)]
    public bool SetRemoteFunctionParameters { get; set; }

    /// <summary>
    /// Fully qualified name of the user-provided connection object which holds the authentication information to send requests to the remote service.
    /// </summary>
    /// <example>projects/{projectId}/locations/{locationId}/connections/{connectionId}</example>
    [UIHint(nameof(SetRemoteFunctionParameters), "", true)]
    public string RemoteConnection { get; set; }

    /// <summary>
    /// Endpoint of the user-provided remote service.
    /// </summary>
    /// <example>https://us-east1-my_gcf_project.cloudfunctions.net/remote_add</example>
    [UIHint(nameof(SetRemoteFunctionParameters), "", true)]
    public string RemoteEndpoint { get; set; }

    /// <summary>
    /// Max number of rows in each batch sent to the remote service. 
    /// If 0, BigQuery dynamically decides the number of rows in a batch.
    /// </summary>
    /// <example>0</example>
    [UIHint(nameof(SetRemoteFunctionParameters), "", true)]
    public long RemoteMaxBatchingRows { get; set; }

    /// <summary>
    /// User-defined context as a set of key/value pairs, which will be sent as function invocation context together with batched arguments in the requests to the remote service. 
    /// The total number of bytes of keys and values must be less than 8KB.
    /// </summary>
    /// <example>[ { foo, bar }, { foo2, bar2 } ]</example>
    [UIHint(nameof(SetRemoteFunctionParameters), "", true)]
    public UserDefinedContext[] RemoteUserDefinedContext { get; set; }
    #endregion Routine specific

    #region Table specific
    /// <summary>
    /// If set to true, queries over this table require a partition filter that can be used for partition elimination to be specified.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(Resource), "", Resources.Table)]
    [DefaultValue(false)]
    public bool RequirePartitionFilter { get; set; }

    /// <summary>
    /// Describes the schema of this table.
    /// </summary>
    /// <example>[ { Name = "Name", Type = "STRING" }, { Name = "AGE", Type = "INTEGER" }, { Name = "BDAY", Type = "DATE" } ]</example>
    [UIHint(nameof(Resource), "", Resources.Table)]
    public TableSchemaParameters[] TableSchema { get; set; }

    /// <summary>
    /// Partition option.
    /// </summary>
    /// <example>TablePartitionOptions.RangePartitioning</example>
    [UIHint(nameof(Resource), "", Resources.Table)]
    [DefaultValue(TablePartitionOptions.None)]
    public TablePartitionOptions Partition { get; set; }

    /// <summary>
    /// The table is partitioned by this field. 
    /// The field must be a top-level NULLABLE/REQUIRED field. 
    /// The only supported type is INTEGER/INT64.
    /// </summary>
    /// <example>AGE</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(Partition), "", TablePartitionOptions.RangePartitioning)]
    public string RangeField { get; set; }

    /// <summary>
    /// The start of range partitioning, inclusive
    /// </summary>
    /// <example>1</example>
    [UIHint(nameof(Partition), "", TablePartitionOptions.RangePartitioning)]
    public long RangeStart { get; set; }

    /// <summary>
    /// The end of range partitioning, exclusive.
    /// </summary>
    /// <example>2</example>
    [UIHint(nameof(Partition), "", TablePartitionOptions.RangePartitioning)]
    public long RangeEnd { get; set; }

    /// <summary>
    /// The width of each interval.
    /// </summary>
    /// <example>2</example>
    [UIHint(nameof(Partition), "", TablePartitionOptions.RangePartitioning)]
    public long RangeInterval { get; set; }

    /// <summary>
    /// If not set, the table is partitioned by pseudo column, referenced via either '_PARTITIONTIME' as TIMESTAMP type, or '_PARTITIONDATE' as DATE type. 
    /// If field is specified, the table is instead partitioned by this field. 
    /// The field must be a top-level TIMESTAMP or DATE field. Its mode must be NULLABLE or REQUIRED.
    /// </summary>
    /// <example>TIMESTAMP</example>
    [UIHint(nameof(Partition), "", TablePartitionOptions.TimePartitioning)]
    public string TimeField { get; set; }

    /// <summary>
    /// Generate one partition per day, hour, month, or year.
    /// </summary>
    /// <example>TimeTypeOptions.DAY</example>
    [DefaultValue(TimeTypeOptions.DAY)]
    [UIHint(nameof(Partition), "", TablePartitionOptions.TimePartitioning)]
    public TimeTypeOptions Type { get; set; }

    /// <summary>
    /// RequirePartitionFilter
    /// </summary>
    /// <example>false</example>
    [DefaultValue("false")]
    [UIHint(nameof(Partition), "", TablePartitionOptions.TimePartitioning)]
    public bool TimeRequirePartitionFilter { get; set; }

    /// <summary>
    /// Number of milliseconds for which to keep the storage for partitions in the table. 
    /// The storage in a partition will have an expiration time of its partition time plus this value.
    /// </summary>
    /// <example>0</example>
    [DefaultValue(0)]
    [UIHint(nameof(Partition), "", TablePartitionOptions.TimePartitioning)]
    public long ExpirationMs { get; set; }
    #endregion Table specific
}