using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.PatchResource.Definitions;

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
    /// Resource description. (Optional)
    /// </summary>
    /// <example>Example description.</example>
    public string Description { get; set; }

    /// <summary>
    /// Friendly name for Dataset or Table resource. (Optional)
    /// </summary>
    /// <example>Example name</example>
    public string FriendlyName { get; set; }

    /// <summary>
    /// The geographic location where the resource should reside. 
    /// See details at https://cloud.google.com/bigquery/docs/locations
    /// </summary>
    /// <example>europe-north1</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("EU")]
    public string Location { get; set; }

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